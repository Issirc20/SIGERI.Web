using Microsoft.Extensions.Configuration;
using SIGERI.Application.Interfaces;
using SIGERI.Application.Usuarios.Security;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Domain.ValueObjects;

namespace SIGERI.Infrastructure.Persistence;

/// <summary>
/// Datos de referencia invariables que deben existir en toda instalación de SIGERI.
/// Todos los bloques son idempotentes: comprueban existencia antes de insertar.
/// </summary>
public static class SeedData
{
    public static void EnsureSeeded(ISigeriDbContext context, IConfiguration? configuration = null)
    {
        SeedOrganizacion(context);
        SeedCategoriasAmenaza(context);
        SeedActivos(context);
        SeedDependencias(context);
        SeedPerfilesCriticos(context);
        SeedControlesMadurezCisV8(context);
        SeedAmenazasMagerit(context);
        SeedVulnerabilidades(context);
        SeedUsuarios(context, configuration);
        SeedAnalyticsExamples(context);
        context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static void SeedOrganizacion(ISigeriDbContext ctx)
    {
        if (ctx.Organizaciones.Any()) return;
        ctx.Organizaciones.Add(new Organizacion("SIGERI Corp", "SIGERI", new List<Activo>())
        {
            CreadoPor = "seed", FechaCreacion = DateTime.UtcNow, Activo = true
        });
        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static void SeedCategoriasAmenaza(ISigeriDbContext ctx)
    {
        var categorias = new[]
        {
            ("CAT-01", "Desastres naturales",          "Eventos naturales fuera del control humano."),
            ("CAT-02", "Origen industrial",             "Accidentes industriales o fallos de infraestructura."),
            ("CAT-03", "Errores y fallos no intencionados","Errores humanos, bugs, configuraciones erróneas."),
            ("CAT-04", "Ataques intencionados",         "Amenazas deliberadas: ciberataques, fraude, espionaje."),
        };

        foreach (var (codigo, nombre, desc) in categorias)
        {
            if (ctx.CategoriasAmenaza.Any(c => c.Nombre == nombre)) continue;
            ctx.CategoriasAmenaza.Add(new CategoriaAmenaza(nombre, desc, new List<Amenaza>())
            {
                FechaCreacion = DateTime.UtcNow, Activo = true
            });
        }
        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static void SeedActivos(ISigeriDbContext ctx)
    {
        if (ctx.Activos.Any()) return;
        var orgId = ctx.Organizaciones.Select(x => x.Id).First();
        var org   = ctx.Organizaciones.First();

        var activos = new[]
        {
            new Activo("ACT-001","SCADA Planta Norte","Sistema de control industrial SCADA principal",TipoActivo.Software,"Jefe OT","Planta Norte",Criticidad.Critica,true,orgId,org,new List<Evaluacion>()),
            new Activo("ACT-002","Servidor IAM",      "Servidor de identidad y gestión de accesos",    TipoActivo.Software,"CISO","CPD",           Criticidad.Alta,   true,orgId,org,new List<Evaluacion>()),
            new Activo("ACT-003","PLC de Bombeo",     "Controlador lógico programable de bombeo",      TipoActivo.Hardware,"Jefe OT","Planta",      Criticidad.Critica,true,orgId,org,new List<Evaluacion>()),
            new Activo("ACT-004","Red OT/IT",         "Infraestructura de red de operaciones",         TipoActivo.Redes,   "Gerente TI","CPD",      Criticidad.Alta,   true,orgId,org,new List<Evaluacion>()),
            new Activo("ACT-005","Backup Corporativo","Sistema de respaldo de datos empresariales",    TipoActivo.Datos,   "CISO","CPD",            Criticidad.Alta,   true,orgId,org,new List<Evaluacion>()),
        };

        foreach (var a in activos)
        {
            a.CreadoPor      = "seed";
            a.FechaCreacion  = DateTime.UtcNow;
            a.Activo         = true;
        }
        ctx.Activos.AddRange(activos);
        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static void SeedDependencias(ISigeriDbContext ctx)
    {
        if (ctx.DependenciasActivos.Any()) return;
        var origen  = ctx.Activos.FirstOrDefault(x => x.Codigo == "ACT-001");
        var destino = ctx.Activos.FirstOrDefault(x => x.Codigo == "ACT-003");
        if (origen is null || destino is null) return;

        ctx.DependenciasActivos.Add(new DependenciaActivo(
            origen.Id, origen, destino.Id, destino,
            TipoDependenciaActivo.Energetica,
            "El SCADA depende operativamente del PLC para el control de bombeo.", 5)
        { CreadoPor = "seed", FechaCreacion = DateTime.UtcNow, Activo = true });

        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static void SeedPerfilesCriticos(ISigeriDbContext ctx)
    {
        if (ctx.PerfilesActivosCriticos.Any()) return;
        var activo = ctx.Activos.FirstOrDefault(x => x.Codigo == "ACT-002");
        if (activo is null) return;

        ctx.PerfilesActivosCriticos.Add(new PerfilActivoCritico(
            activo.Id, activo,
            NivelImpactoOctave.Alto, NivelImpactoOctave.Alto,
            NivelImpactoOctave.Medio, NivelImpactoOctave.Alto, NivelImpactoOctave.Alto,
            "Repositorio de identidades, SIEM y respaldos automáticos",
            "CPD principal — Sala 3 Nivel 2",
            "CISO, Analista SOC, Administrador IAM")
        { CreadoPor = "seed", FechaCreacion = DateTime.UtcNow, Activo = true });

        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    /// <summary>Catálogo oficial CIS Controls v8 — los 18 controles completos.</summary>
    private static void SeedControlesMadurezCisV8(ISigeriDbContext ctx)
    {
        var catalogo = new[]
        {
            ("CIS-01","Inventario y control de activos empresariales",   FuncionNist.Identify, 2,4,"Gestión activa de todos los activos empresariales conectados a la infraestructura."),
            ("CIS-02","Inventario y control de activos de software",     FuncionNist.Identify, 2,4,"Gestión activa de todo el software instalado para que solo software autorizado esté en uso."),
            ("CIS-03","Protección de datos",                             FuncionNist.Protect,  2,4,"Desarrollar procesos para identificar, clasificar, manejar, retener y eliminar datos de forma segura."),
            ("CIS-04","Configuración segura de activos empresariales",   FuncionNist.Protect,  2,4,"Establecer y mantener la configuración segura de activos de hardware y software."),
            ("CIS-05","Gestión de cuentas",                              FuncionNist.Govern,   3,4,"Usar procesos y herramientas para asignar y gestionar autorización en credenciales de cuentas."),
            ("CIS-06","Gestión del control de acceso",                   FuncionNist.Protect,  2,4,"Usar procesos y herramientas para crear, asignar, gestionar y revocar credenciales de acceso."),
            ("CIS-07","Gestión continua de vulnerabilidades",            FuncionNist.Identify, 2,4,"Desarrollar un plan para evaluar y corregir continuamente vulnerabilidades en activos."),
            ("CIS-08","Gestión de registros de auditoría",               FuncionNist.Detect,   1,4,"Recopilar, alertar, revisar y retener registros de auditoría que permitan detectar ataques."),
            ("CIS-09","Protección del correo electrónico y del navegador",FuncionNist.Protect, 2,4,"Mejorar las protecciones para minimizar la superficie de ataque y los riesgos."),
            ("CIS-10","Defensas contra malware",                         FuncionNist.Protect,  2,4,"Prevenir o controlar la instalación, propagación y ejecución de aplicaciones maliciosas."),
            ("CIS-11","Recuperación de datos",                           FuncionNist.Recover,  1,4,"Establecer y mantener prácticas para una correcta recuperación de datos."),
            ("CIS-12","Gestión de la infraestructura de red",            FuncionNist.Protect,  2,4,"Establecer, implementar y gestionar activamente la infraestructura de red."),
            ("CIS-13","Monitoreo y defensa de red",                      FuncionNist.Detect,   1,4,"Operar procesos y herramientas para establecer y mantener una defensa integral de red."),
            ("CIS-14","Concienciación y formación en seguridad",         FuncionNist.Govern,   1,4,"Establecer y mantener un programa de concienciación de seguridad para mitigar riesgos."),
            ("CIS-15","Gestión de proveedores de servicios",             FuncionNist.Govern,   1,4,"Desarrollar un proceso para evaluar a los proveedores de servicios que manejan datos sensibles."),
            ("CIS-16","Seguridad del software de aplicación",            FuncionNist.Protect,  2,4,"Gestionar el ciclo de vida de seguridad del software desarrollado o adquirido."),
            ("CIS-17","Gestión de respuesta a incidentes",               FuncionNist.Respond,  1,4,"Establecer un programa para preparar, detectar, contener y recuperarse de incidentes."),
            ("CIS-18","Pruebas de penetración",                          FuncionNist.Identify, 1,4,"Probar la solidez de las defensas mediante simulaciones de ataques reales."),
        };

        foreach (var (codigo, nombre, funcion, nivelActual, nivelObjetivo, descripcion) in catalogo)
        {
            if (ctx.ControlesMadurez.Any(c => c.Codigo == codigo)) continue;
            ctx.ControlesMadurez.Add(new ControlMadurez(codigo, nombre, funcion, nivelActual, nivelObjetivo, descripcion)
            { CreadoPor = "seed", FechaCreacion = DateTime.UtcNow, Activo = true });
        }
        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    /// <summary>Catálogo MAGERIT v3 — amenazas representativas por categoría.</summary>
    private static void SeedAmenazasMagerit(ISigeriDbContext ctx)
    {
        var categoriaId = ctx.CategoriasAmenaza
            .Where(c => c.Nombre == "Ataques intencionados")
            .Select(c => c.Id).FirstOrDefault();
        if (categoriaId == Guid.Empty) return;
        var categoria = ctx.CategoriasAmenaza.First(c => c.Id == categoriaId);

        var amenazas = new[]
        {
            ("A.01","Ransomware SCADA/ICS",   "Cifrado de sistemas de control industrial para extorsión."),
            ("A.02","Phishing dirigido (BEC)","Suplantación de identidad para fraude financiero o acceso."),
            ("A.03","Manipulación de PLC",    "Modificación maliciosa de lógica de controladores ICS."),
            ("A.04","Ataque DDoS",            "Denegación de servicio distribuido sobre infraestructura crítica."),
            ("A.05","Acceso no autorizado",   "Intrusión por credenciales comprometidas o escalada de privilegios."),
            ("A.06","Fuga de datos",          "Exfiltración de información sensible o confidencial."),
            ("A.07","Ingeniería social",      "Manipulación psicológica para obtener información o acceso."),
            ("A.08","Man-in-the-Middle",      "Interceptación de comunicaciones entre sistemas."),
        };

        foreach (var (codigo, nombre, desc) in amenazas)
        {
            if (ctx.Amenazas.Any(a => a.Codigo == codigo)) continue;
            ctx.Amenazas.Add(new Amenaza(codigo, nombre, desc, categoriaId, categoria, new List<Riesgo>())
            { CreadoPor = "seed", FechaCreacion = DateTime.UtcNow, Activo = true });
        }
        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static void SeedVulnerabilidades(ISigeriDbContext ctx)
    {
        var vulnerabilidades = new[]
        {
            ("V.01","Contraseñas débiles o por defecto",          "Alta",  "Uso de contraseñas predecibles o sin cambiar desde instalación."),
            ("V.02","Software sin parches de seguridad",          "Alta",  "Sistemas operativos y aplicaciones con vulnerabilidades conocidas sin corregir."),
            ("V.03","Acceso remoto sin MFA",                      "Alta",  "Conexiones remotas sin autenticación multifactor habilitada."),
            ("V.04","Comunicaciones no cifradas",                 "Media", "Transmisión de datos sensibles en texto plano sobre redes internas o externas."),
            ("V.05","Falta de segmentación de red OT/IT",         "Alta",  "Redes operacionales y corporativas sin separación adecuada."),
            ("V.06","Falta de registros de auditoría",            "Media", "Ausencia de logs que permitan detectar y analizar incidentes."),
            ("V.07","Configuración incorrecta de firewall",       "Alta",  "Reglas permisivas o erróneas que exponen servicios internos."),
            ("V.08","Personal sin formación en seguridad",        "Media", "Empleados que no reconocen técnicas de ingeniería social o phishing."),
        };

        foreach (var (codigo, nombre, severidad, desc) in vulnerabilidades)
        {
            if (ctx.Vulnerabilidades.Any(v => v.Codigo == codigo)) continue;
            ctx.Vulnerabilidades.Add(new Vulnerabilidad(codigo, nombre, desc, severidad, new List<Riesgo>())
            { CreadoPor = "seed", FechaCreacion = DateTime.UtcNow, Activo = true });
        }
        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static void SeedUsuarios(ISigeriDbContext ctx, IConfiguration? configuration)
    {
        var adminCorreo = configuration?["DevCredentials:AdminCorreo"];
        var adminPassword = configuration?["DevCredentials:AdminPassword"];

        if (!string.IsNullOrWhiteSpace(adminCorreo) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            SeedUsuario(
                ctx,
                adminCorreo,
                adminPassword,
                "Administrador",
                "SIGERI",
                RolUsuario.Administrador);
        }

        var analistaCorreo = configuration?["DevCredentials:AnalistaCorreo"];
        var analistaPassword = configuration?["DevCredentials:AnalistaPassword"];
        if (!string.IsNullOrWhiteSpace(analistaCorreo) && !string.IsNullOrWhiteSpace(analistaPassword))
        {
            SeedUsuario(
                ctx,
                analistaCorreo,
                analistaPassword,
                "Analista",
                "Riesgos",
                RolUsuario.AnalistaRiesgos);
        }
    }

    private static void SeedUsuario(
        ISigeriDbContext ctx,
        string correo,
        string? password,
        string nombre,
        string apellido,
        RolUsuario rol)
    {
        if (string.IsNullOrWhiteSpace(password) || ctx.Usuarios.Any(u => u.Correo == correo)) return;

        ctx.Usuarios.Add(new Usuario(
            nombre, apellido, correo,
            PasswordHashService.HashPassword(password),
            rol, true,
            new List<Evaluacion>())
        { Id = Guid.NewGuid(), CreadoPor = "seed", FechaCreacion = DateTime.UtcNow, Activo = true });
    }

    private static void SeedAnalyticsExamples(ISigeriDbContext ctx)
    {
        var activeUsers = ctx.Usuarios.Where(u => u.Estado).ToList();
        var admin = ctx.Usuarios.FirstOrDefault(u => u.Rol == RolUsuario.Administrador && u.Estado)
                    ?? activeUsers.FirstOrDefault();
        var analista = ctx.Usuarios.FirstOrDefault(u => u.Rol == RolUsuario.AnalistaRiesgos && u.Estado)
                       ?? activeUsers.Skip(1).FirstOrDefault()
                       ?? admin;

        SeedScenarioForUser(
            ctx,
            admin,
            "ACT-001",
            "A.01",
            "V.02",
            "[SEED-ANALYTICS] Evaluación prioritaria SCADA",
            5,
            5,
            EstadoPlan.EnProgreso,
            870000m,
            300000m,
            79m,
            "Contención de red OT y hardening de PLC");

        SeedScenarioForUser(
            ctx,
            analista,
            "ACT-002",
            "A.05",
            "V.03",
            "[SEED-ANALYTICS] Evaluación IAM y accesos remotos",
            4,
            4,
            EstadoPlan.Completado,
            280000m,
            95000m,
            55m,
            "MFA obligatorio y PAM para cuentas privilegiadas");
    }

    private static void SeedScenarioForUser(
        ISigeriDbContext ctx,
        Usuario? usuario,
        string activoCodigo,
        string amenazaCodigo,
        string vulnerabilidadCodigo,
        string observacion,
        int probabilidad,
        int impacto,
        EstadoPlan estadoPlan,
        decimal aleBase,
        decimal costoSalvaguarda,
        decimal porcentajeMitigacion,
        string estrategia)
    {
        if (usuario is null) return;
        if (ctx.Evaluaciones.Any(e => e.UsuarioId == usuario.Id && e.Observaciones == observacion)) return;

        var activo = ctx.Activos.FirstOrDefault(a => a.Codigo == activoCodigo);
        var amenaza = ctx.Amenazas.FirstOrDefault(a => a.Codigo == amenazaCodigo);
        var vulnerabilidad = ctx.Vulnerabilidades.FirstOrDefault(v => v.Codigo == vulnerabilidadCodigo);
        if (activo is null || amenaza is null || vulnerabilidad is null) return;

        var evaluacion = new Evaluacion(
            DateTime.UtcNow,
            observacion,
            usuario.Id,
            usuario,
            activo.Id,
            activo,
            new List<Riesgo>())
        {
            CreadoPor = "seed",
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        var puntaje = probabilidad * impacto;
        var riesgo = new Riesgo(
            "Riesgo operativo cibernético",
            "Escenario de riesgo sembrado para analítica de dashboard.",
            new Probabilidad(probabilidad, ObtenerDescripcionProbabilidad(probabilidad)),
            new Impacto(impacto, ObtenerDescripcionImpacto(impacto)),
            puntaje,
            ObtenerNivelRiesgo(puntaje),
            EstadoRiesgo.EnTratamiento,
            evaluacion.Id,
            evaluacion,
            amenaza.Id,
            amenaza,
            vulnerabilidad.Id,
            vulnerabilidad,
            new List<PlanTratamiento>(),
            new List<HistorialRiesgoResidual>())
        {
            CreadoPor = "seed",
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        var plan = new PlanTratamiento(
            riesgo.Id,
            riesgo,
            estrategia,
            "Plan de mitigación sembrado para visualizar KPIs por usuario.",
            DateTime.UtcNow.AddDays(-15),
            DateTime.UtcNow.AddDays(45),
            estadoPlan,
            new List<Control>())
        {
            AleBase = aleBase,
            CostoSalvaguarda = costoSalvaguarda,
            PorcentajeMitigacion = porcentajeMitigacion,
            CreadoPor = "seed",
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        var control = new Control(
            plan.Id,
            plan,
            "CTRL-SEED-01",
            "Control de mitigación demo",
            "Control de referencia para visualizar avances en el dashboard.",
            true,
            true)
        {
            CreadoPor = "seed",
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        plan.Controles.Add(control);
        riesgo.PlanesTratamiento.Add(plan);
        evaluacion.Riesgos.Add(riesgo);

        ctx.Evaluaciones.Add(evaluacion);
        ctx.Riesgos.Add(riesgo);
        ctx.PlanesTratamiento.Add(plan);
        ctx.Controles.Add(control);

        ctx.SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static string ObtenerNivelRiesgo(int puntajeRiesgo) => puntajeRiesgo switch
    {
        <= 4 => "Bajo",
        <= 9 => "Medio",
        <= 15 => "Alto",
        <= 25 => "Crítico",
        _ => "Bajo"
    };

    private static string ObtenerDescripcionProbabilidad(int valor) => valor switch
    {
        1 => "Muy baja",
        2 => "Baja",
        3 => "Media",
        4 => "Alta",
        5 => "Muy alta",
        _ => "Media"
    };

    private static string ObtenerDescripcionImpacto(int valor) => valor switch
    {
        1 => "Muy bajo",
        2 => "Bajo",
        3 => "Medio",
        4 => "Alto",
        5 => "Muy alto",
        _ => "Medio"
    };
}

// ui.js - theme & utilities (Fase 2)
(function(){
    const root = document.documentElement;
    const sidebar = document.getElementById('sidebar');
    const layoutWrapper = document.getElementById('layoutWrapper');
    const footer = document.getElementById('footer');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarCollapse = document.getElementById('sidebarCollapse');

    const savedTheme = localStorage.getItem('sig_theme');

    function applyTheme(theme){
        if(theme === 'light') root.classList.add('light');
        else root.classList.remove('light');
    }

    if(savedTheme) applyTheme(savedTheme);
    else applyTheme('dark');

    window.SIGERI = window.SIGERI || {};
    window.SIGERI.toggleTheme = function(){
        const isLight = root.classList.toggle('light');
        localStorage.setItem('sig_theme', isLight ? 'light' : 'dark');
    };

    function setSidebarCollapsed(collapsed){
        if(!sidebar) return;
        if(collapsed){
            sidebar.classList.add('collapsed');
            layoutWrapper.classList.add('collapsed');
            footer.classList.add('collapsed');
        } else {
            sidebar.classList.remove('collapsed');
            layoutWrapper.classList.remove('collapsed');
            footer.classList.remove('collapsed');
        }
        localStorage.setItem('sidebarCollapsed', collapsed);
    }

    if(sidebarToggle){
        sidebarToggle.addEventListener('click', ()=>{
            const collapsed = !sidebar.classList.contains('collapsed');
            setSidebarCollapsed(collapsed);
        });
    }

    if(sidebarCollapse){
        sidebarCollapse.addEventListener('click', ()=>{
            const collapsed = !sidebar.classList.contains('collapsed');
            setSidebarCollapsed(collapsed);
        });
    }

    // Restore state
    if(localStorage.getItem('sidebarCollapsed') === 'true'){
        setSidebarCollapsed(true);
    }

    // Current date
    const dateEl = document.getElementById('currentDate');
    function updateDate(){
        if(!dateEl) return;
        const now = new Date();
        dateEl.textContent = now.toLocaleString();
    }
    updateDate();
    setInterval(updateDate, 60*1000);

    // Notifications placeholder
    const btnNotifications = document.getElementById('btnNotifications');
    if(btnNotifications){
        btnNotifications.addEventListener('click', ()=>{
            // simple placeholder action: toggle badge
            const badge = document.getElementById('notifBadge');
            if(badge) badge.classList.add('hidden');
            alert('No hay notificaciones nuevas.');
        });
    }

    // Search interaction (enter to go to search page)
    const globalSearch = document.getElementById('globalSearch');
    if(globalSearch){
        globalSearch.addEventListener('keypress', (e)=>{
            if(e.key === 'Enter'){
                const q = globalSearch.value.trim();
                if(q.length) window.location.href = '/Search?q=' + encodeURIComponent(q);
            }
        });
    }

    // Initialize lucide icons replacement if available
    document.addEventListener('DOMContentLoaded', ()=>{ if(window.lucide) lucide.replace(); });

    // Micro-animations for dashboard cards (Motion One if available)
    document.addEventListener('DOMContentLoaded', ()=>{
        const cards = Array.from(document.querySelectorAll('.dashboard-card, .sig-card'));
        if(cards.length === 0) return;
        if(window.motion){
            motion.animate(cards, { opacity: [0,1], transform: ['translateY(16px)','translateY(0px)'] }, { duration: 600, easing: 'cubic-bezier(.2,.9,.2,1)', delay: 0, stagger: 80 });
        } else {
            cards.forEach((el,i)=>{
                el.style.opacity = 0;
                el.style.transform = 'translateY(12px)';
                setTimeout(()=>{
                    el.style.transition = 'opacity .45s ease, transform .45s ease';
                    el.style.opacity = 1;
                    el.style.transform = 'translateY(0)';
                }, i*80);
            });
        }
    });

})();

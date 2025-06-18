document.addEventListener('DOMContentLoaded', function () {
    try {
        // Validar que existan los elementos antes de operar
        var notificacionesList = document.getElementById('notificaciones-list');
        var notificacionesBadge = document.getElementById('notificaciones-badge');
        var notificacionesDropdown = document.getElementById('notificacionesDropdown');
        if (!notificacionesList || !notificacionesBadge || !notificacionesDropdown) {
            // Si falta alguno, no ejecutar nada
            return;
        }

        //console.log("Script de notificaciones cargado");

        async function cargarNotificaciones() {
            const resp = await fetch(window.notificacionesUrls.getNotificaciones);
            const notifs = await resp.json();
            //console.log("Notificaciones recibidas:", notifs);
            notificacionesList.innerHTML = '';
            if (notifs.length === 0) {
                notificacionesList.className = "text-center text-muted py-2";
                notificacionesList.innerHTML = 'Sin notificaciones';
            } else {
                notificacionesList.className = "";
                notifs.forEach(n => {
                    let href = n.urlDestino || '#';
                    // Si no es absoluta ni comienza con "/", la convertimos a absoluta
                    if (href && !href.startsWith('http') && !href.startsWith('/')) {
                        href = window.location.origin + '/' + href.replace(/^\/+/, '');
                    } else if (href && href.startsWith('/')) {
                        href = window.location.origin + href;
                    }
                    const a = document.createElement('a');
                    a.href = href;
                    a.onclick = () => { marcarNotificacionLeida(n.idNotificacion); };
                    a.innerHTML = `${n.titulo}<br/><small>${n.detalle}</small>`;
                    a.className = 'dropdown-item' + (n.read ? '' : ' fw-bold');
                    notificacionesList.appendChild(a);
                });
            }
            // Badge
            const countResp = await fetch(window.notificacionesUrls.cantidadNoLeidas);
            const count = (await countResp.json()).count;
            // Limitar el número mostrado en el badge
            notificacionesBadge.textContent = count > 99 ? '99+' : count;
            notificacionesBadge.style.display = count > 0 ? 'inline-block' : 'none';
        }

        async function marcarNotificacionLeida(id) {
            await fetch(window.notificacionesUrls.marcarComoLeida, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': (window.$ && $('input[name="__RequestVerificationToken"]').val()) || ''
                },
                body: JSON.stringify({ id })
            });
            setTimeout(cargarNotificaciones, 500);
        }

        cargarNotificaciones();
        notificacionesDropdown.addEventListener('show.bs.dropdown', cargarNotificaciones);

    } catch (e) {
        console.warn('navbar.js error:', e);
    }
});

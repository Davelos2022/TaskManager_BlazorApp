//For App
window.toggleBodyClass = {
    addModalOpen: function () {
        document.body.classList.add('modal-open');
    },
    removeModalOpen: function () {
        document.body.classList.remove('modal-open');
    }
};

function resetModalScroll() {
    var modalBodies = document.getElementsByClassName('modal-body');
    if (modalBodies) {
        for (var i = 0; i < modalBodies.length; i++) {
            modalBodies[i].scrollTop = 0;
        }
    }
}

function initializeDragAndDrop(dotnetHelper) {
    ;
    var el = document.getElementById("tasksList");
    if (el) {
        if (window.sortableInstance) {
            window.sortableInstance.destroy();
        }

        window.sortableInstance = Sortable.create(el, {
            animation: 150,
            delay: 100,
            ghostClass: "sortable-ghost",
            onEnd: function (evt) {
                var order = [];
                [...el.children].forEach(row => {
                    order.push(row.getAttribute("data-task-id"));
                });
                console.log("New order: ", order);
                dotnetHelper.invokeMethodAsync("UpdateTasksOrder", order);
            }
        });
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const starsContainer = document.getElementById('stars');
    const numberOfStars = 150;

    for (let i = 0; i < numberOfStars; i++) {
        const star = document.createElement('div');
        star.classList.add('star');
        star.style.top = `${Math.random() * 100}%`;
        star.style.left = `${Math.random() * 100}%`;
        star.style.animationDelay = `${Math.random() * 3}s`;
        starsContainer.appendChild(star);
    }
});

//For TG App
window.onload = function () {
    Telegram.WebApp.ready();
};

//For TimeOffSetZone
window.getTimeZoneOffset = () => new Date().getTimezoneOffset();
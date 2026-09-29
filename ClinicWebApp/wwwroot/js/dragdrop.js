window.dragDrop = {
    init: function () {
        document.addEventListener('dragstart', function (e) {
            if (e.target.hasAttribute('draggable') && e.target.getAttribute('draggable') === 'true') {
                e.dataTransfer.effectAllowed = 'move';
            }
        });
        document.addEventListener('dragover', function (e) {
            e.preventDefault();
        });
        document.addEventListener('drop', function (e) {
            e.preventDefault();
        });
    }
};
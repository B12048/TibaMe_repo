document.getElementById('uploadForm')
    .addEventListener('submit', async function (e) {
        e.preventDefault();
        const formData = new FormData(this);
        const response = await fetch('/api/imageuploadapi/upload', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();
        if (result.success) {
            const img = document.getElementById('Portrait');
            img.src = result.url;
        }
    });
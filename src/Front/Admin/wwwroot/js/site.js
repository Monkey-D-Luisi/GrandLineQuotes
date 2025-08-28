function openQuoteModal(id = null) {
    let url = '/quotes/form';
    if (id) url += '/' + id;

    fetch(url)
        .then(res => res.text())
        .then(html => {
            document.getElementById('quoteModalContent').innerHTML = html;
            new bootstrap.Modal(document.getElementById('quoteModal')).show();
            attachVideoInputPreview();
            attachNewOptionInputs()
        })
        .catch(error => {
            console.error('Error loading quote form:', error);
            alert('Error loading quote form');
        });
}

function attachVideoInputPreview() {
    setupVideoInputPreview("videoInput", "videoPreview");
    setupVideoInputPreview("videoInputEs", "videoPreviewEs");
}

function setupVideoInputPreview(inputId, previewId) {
    const input = document.getElementById(inputId);
    if (!input) return;

    input.addEventListener("change", function () {
        if (this.files && this.files[0]) {
            const file = this.files[0];
            const blobUrl = URL.createObjectURL(file);

            let video = document.getElementById(previewId);

            if (!video) {
                const videoContainer = document.createElement("div");
                videoContainer.className = "mb-3";

                const label = document.createElement("label");
                label.className = "form-label";
                label.innerText = "Video preview";

                video = document.createElement("video");
                video.id = previewId;
                video.width = 320;
                video.height = 240;
                video.controls = true;

                const source = document.createElement("source");
                source.src = blobUrl;
                source.type = file.type;

                video.appendChild(source);
                videoContainer.appendChild(label);
                videoContainer.appendChild(video);

                input.parentElement.insertBefore(videoContainer, input);
            } else {
                const source = video.querySelector("source");
                if (source) {
                    source.src = blobUrl;
                    video.load();
                }
            }
        }
    });
}

function attachNewOptionInputs() {
    const authorSelect = document.getElementById("AuthorSelect");
    const newAuthorInput = document.getElementById("NewAuthorInput");
    const episodeSelect = document.getElementById("EpisodeSelect");
    const newEpisodeInput = document.getElementById("NewEpisodeInput");
    const arcSelect = document.getElementById("ArcSelect");
    const newArcInput = document.getElementById("NewArcInput");
    const sagaSelect = document.getElementById("SagaSelect");
    const newSagaInput = document.getElementById("NewSagaInput");

    authorSelect?.addEventListener("change", () => {
        newAuthorInput.style.display = authorSelect.value === "-1" ? "block" : "none";
    });

    episodeSelect?.addEventListener("change", () => {
        newEpisodeInput.style.display = episodeSelect.value === "-1" ? "block" : "none";
    });

    arcSelect?.addEventListener("change", () => {
        newArcInput.style.display = arcSelect.value === "-1" ? "block" : "none";
    });

    sagaSelect?.addEventListener("change", () => {
        newSagaInput.style.display = sagaSelect.value === "-1" ? "block" : "none";
    });
}

function submitQuote() {
    const form = document.getElementById('quoteForm');
    const isReviewed = document.getElementById('isReviewed');
    const formData = new FormData(form);
    formData.append('IsReviewed', isReviewed.checked ? 'true' : 'false');

    fetch('/quotes/form', {
        method: 'POST',
        body: formData
    })
        .then(async response => {
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData?.error ?? errorData?.errors[0] ?? 'Unknown error');
            }
            const modalEl = document.getElementById('quoteModal');
            bootstrap.Modal.getInstance(modalEl)?.hide();
            const filters = document.getElementById('quoteFilters');
            if (filters) {
                const params = new URLSearchParams(new FormData(filters));
                fetch(`?${params.toString()}`, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
                    .then(res => res.text())
                    .then(html => {
                        document.getElementById('quotesList').innerHTML = html;
                        initializeDataTables();
                    });
            } else {
                location.reload();
            }
        })
        .catch(error => {
            console.error('Error saving quote:', error);
            alert(error.message || "An error occurred while trying to save the quote.");
        });

}

function deleteQuote(id) {
    if (!id) {
        console.error('Quote ID is required for deletion.');
        return;
    }

    const url = `/quotes/${id}`;

    if (!confirm("Are you sure you want to delete this quote?")) return;

    fetch(url, {
        method: 'DELETE',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
        .then(async response => {
            if (response.ok) {
                const row = document.querySelector(`button[onclick="openQuoteModal(${id})"]`)?.closest("tr");
                if (row) row.remove();
            } else {
                const errorData = await response.json();
                throw new Error(errorData?.error || 'Unknown error');
            }
        })
        .catch(error => {
            console.error('Error deleting quote:', error);
            alert(error.message || "An error occurred while trying to save the quote.");
        });

}

function openArcModal(id = null) {
    let url = '/arcs/form';
    if (id) url += '/' + id;

    fetch(url)
        .then(res => res.text())
        .then(html => {
            document.getElementById('arcModalContent').innerHTML = html;
            new bootstrap.Modal(document.getElementById('arcModal')).show();
        });
}

function submitArc() {
    const form = document.getElementById('arcForm');
    const formData = new FormData(form);
    fetch('/arcs/form', { method: 'POST', body: formData })
        .then(response => { if (response.ok) location.reload(); else throw new Error(); })
        .catch(error => { console.error('Error saving arc:', error); alert('Error saving arc'); });
}

function deleteArc(id) {
    if (!confirm('Are you sure you want to delete this arc?')) return;
    fetch(`/arcs/${id}`, { method: 'DELETE', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
        .then(response => {
            if (response.ok) {
                const row = document.querySelector(`button[onclick="openArcModal(${id})"]`)?.closest('tr');
                row?.remove();
            } else throw new Error();
        })
        .catch(error => { console.error('Error deleting arc:', error); alert('Error deleting arc'); });
}

function openCharacterModal(id = null) {
    let url = '/characters/form';
    if (id) url += '/' + id;
    fetch(url)
        .then(res => res.text())
        .then(html => {
            document.getElementById('characterModalContent').innerHTML = html;
            new bootstrap.Modal(document.getElementById('characterModal')).show();
        });
}

function submitCharacter() {
    const form = document.getElementById('characterForm');
    const formData = new FormData(form);
    fetch('/characters/form', { method: 'POST', body: formData })
        .then(response => { if (response.ok) location.reload(); else throw new Error(); })
        .catch(error => { console.error('Error saving character:', error); alert('Error saving character'); });
}

function deleteCharacter(id) {
    if (!confirm('Are you sure you want to delete this character?')) return;
    fetch(`/characters/${id}`, { method: 'DELETE', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
        .then(response => {
            if (response.ok) {
                const row = document.querySelector(`button[onclick="openCharacterModal(${id})"]`)?.closest('tr');
                row?.remove();
            } else throw new Error();
        })
        .catch(error => { console.error('Error deleting character:', error); alert('Error deleting character'); });
}

function openEpisodeModal(number = null) {
    let url = '/episodes/form';
    if (number) url += '/' + number;
    fetch(url)
        .then(res => res.text())
        .then(html => {
            document.getElementById('episodeModalContent').innerHTML = html;
            new bootstrap.Modal(document.getElementById('episodeModal')).show();
        });
}

function submitEpisode() {
    const form = document.getElementById('episodeForm');
    const formData = new FormData(form);
    fetch('/episodes/form', { method: 'POST', body: formData })
        .then(response => {
            if (!response.ok) throw new Error();
            const modalEl = document.getElementById('episodeModal');
            bootstrap.Modal.getInstance(modalEl)?.hide();
            const filters = document.getElementById('episodeFilters');
            if (filters) {
                const params = new URLSearchParams(new FormData(filters));
                fetch(`?${params.toString()}`, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
                    .then(res => res.text())
                    .then(html => {
                        document.getElementById('episodesList').innerHTML = html;
                        initializeDataTables();
                    });
            } else {
                location.reload();
            }
        })
        .catch(error => { console.error('Error saving episode:', error); alert('Error saving episode'); });
}

function deleteEpisode(number) {
    if (!confirm('Are you sure you want to delete this episode?')) return;
    fetch(`/episodes/${number}`, { method: 'DELETE', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
        .then(response => {
            if (response.ok) {
                const row = document.querySelector(`button[onclick="openEpisodeModal(${number})"]`)?.closest('tr');
                row?.remove();
            } else throw new Error();
        })
        .catch(error => { console.error('Error deleting episode:', error); alert('Error deleting episode'); });
}

function openSagaModal(id = null) {
    let url = '/sagas/form';
    if (id) url += '/' + id;
    fetch(url)
        .then(res => res.text())
        .then(html => {
            document.getElementById('sagaModalContent').innerHTML = html;
            new bootstrap.Modal(document.getElementById('sagaModal')).show();
        });
}

function submitSaga() {
    const form = document.getElementById('sagaForm');
    const formData = new FormData(form);
    fetch('/sagas/form', { method: 'POST', body: formData })
        .then(response => { if (response.ok) location.reload(); else throw new Error(); })
        .catch(error => { console.error('Error saving saga:', error); alert('Error saving saga'); });
}

function deleteSaga(id) {
    if (!confirm('Are you sure you want to delete this saga?')) return;
    fetch(`/sagas/${id}`, { method: 'DELETE', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
        .then(response => {
            if (response.ok) {
                const row = document.querySelector(`button[onclick="openSagaModal(${id})"]`)?.closest('tr');
                row?.remove();
            } else throw new Error();
        })
        .catch(error => { console.error('Error deleting saga:', error); alert('Error deleting saga'); });
}

function initializeDataTables() {
    ['#quotesTable', '#episodesTable'].forEach(selector => {
        if ($.fn.DataTable.isDataTable(selector)) {
            $(selector).DataTable().destroy();
        }
        if ($(selector).length) {
            $(selector).DataTable();
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {
    function debounce(fn, delay) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => fn.apply(this, args), delay);
        };
    }

    const quoteForm = document.getElementById('quoteFilters');
    if (quoteForm) {
        const authorFilter = document.getElementById('authorFilter');
        const arcFilter = document.getElementById('arcFilter');
        const searchInput = document.getElementById('searchTerm');
        const isReviewedFilter = document.getElementById('isReviewedFilter');

        function submitQuoteFilters() {
            const params = new URLSearchParams(new FormData(quoteForm));
            fetch(`?${params.toString()}`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(res => res.text())
                .then(html => {
                    document.getElementById('quotesList').innerHTML = html;
                    initializeDataTables();
                });
        }

        authorFilter.addEventListener('change', submitQuoteFilters);
        arcFilter.addEventListener('change', submitQuoteFilters);
        searchInput.addEventListener('input', debounce(submitQuoteFilters, 300));
        isReviewedFilter.addEventListener('change', submitQuoteFilters);
    }

    const episodeForm = document.getElementById('episodeFilters');
    if (episodeForm) {
        const arcFilter = document.getElementById('episodeArcFilter');
        arcFilter.addEventListener('change', function () {
            const params = new URLSearchParams(new FormData(episodeForm));
            fetch(`?${params.toString()}`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(res => res.text())
                .then(html => {
                    document.getElementById('episodesList').innerHTML = html;
                    initializeDataTables();
                });
        });
    }
    initializeDataTables();
});
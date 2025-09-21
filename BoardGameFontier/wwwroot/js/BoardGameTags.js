function initBoardGameTags() {
    const select = document.getElementById("category-select");
    const tagContainer = document.getElementById("selected-tags");
    const hiddenInput = document.getElementById("hidden-tags");
    if (!select || !tagContainer || !hiddenInput) return; // 防呆
    if (select.dataset.binded) return;
    select.dataset.binded = "1";

    // --- 這份 selectedSet 放在外面，初始化+新增+移除都用它 ---
    const selectedSet = new Set();

    // ----------- 這裡做初始化(載入舊標籤) -----------
    var selected = window.boardGameTags;
    if (selected && selected.length > 0) {
        selected.forEach(function (tag) {
            addTag(tag);
        });
    }

    // ----------- 這裡處理 select 新增 ----------
    select.addEventListener("change", function () {
        const value = this.value;
        if (value && !selectedSet.has(value)) {
            addTag(value);
        }
        this.value = ""; // 選完歸空
    });

    function addTag(value) {
        selectedSet.add(value);
        const tag = document.createElement("span");
        tag.className = "badge bg-success px-3 py-2 d-inline-flex align-items-center";
        tag.style.fontSize = "1rem";
        tag.textContent = value;
        // 刪除按鈕
        const removeBtn = document.createElement("button");
        removeBtn.type = "button";
        removeBtn.className = "btn-close btn-close-white ms-2";
        removeBtn.style.fontSize = "0.7rem";
        removeBtn.setAttribute("aria-label", "Remove");
        removeBtn.addEventListener("click", () => {
            tag.remove();
            selectedSet.delete(value);
            updateHiddenInput();
        });
        tag.appendChild(removeBtn);
        tagContainer.appendChild(tag);
        updateHiddenInput();
    }

    function updateHiddenInput() {
        hiddenInput.value = [...selectedSet].join(',');
    }
}

// ---- 只需要這一句即可！ ----
window.onload = function () {
    initBoardGameTags();
};

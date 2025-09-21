// birthdate-select.js

const yearSelect = document.getElementById("year");
const monthSelect = document.getElementById("month");
const daySelect = document.getElementById("day");
const hiddenBirth = document.getElementById("BirthDate");

const today = new Date();
const minYear = 1910;
// 只允許 12 歲（含）以上
const maxYear = today.getFullYear() - 12;

// 1. 產生年份
for (let y = maxYear; y >= minYear; y--) {
    yearSelect.add(new Option(y, y));
}
// 2. 產生月份
for (let m = 1; m <= 12; m++) {
    monthSelect.add(new Option(m, m));
}

// 3. 判斷閏年
function isLeapYear(year) {
    return (year % 4 === 0 && year % 100 !== 0) || (year % 400 === 0);
}

// 4. 產生日數，並維持舊的日選擇
function updateDays() {
    const y = parseInt(yearSelect.value);
    const m = parseInt(monthSelect.value);
    if (isNaN(y) || isNaN(m)) {
        daySelect.innerHTML = "";
        hiddenBirth.value = "";
        return;
    }
    const daysInMonth = [31, (isLeapYear(y) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    const maxDay = daysInMonth[m - 1];
    const prevDay = parseInt(daySelect.value) || 1;
    daySelect.innerHTML = "";
    for (let d = 1; d <= maxDay; d++) {
        daySelect.add(new Option(d, d));
    }
    daySelect.value = prevDay <= maxDay ? prevDay : maxDay;
    updateBirthDate();
}

// 5. 組合日期到隱藏欄位
function updateBirthDate() {
    const y = yearSelect.value;
    const m = monthSelect.value;
    const d = daySelect.value;
    if (y && m && d) {
        // 月日補零
        const mm = m.padStart ? m.padStart(2, '0') : ('0' + m).slice(-2);
        const dd = d.padStart ? d.padStart(2, '0') : ('0' + d).slice(-2);
        hiddenBirth.value = `${y}-${mm}-${dd}`;
    } else {
        hiddenBirth.value = "";
    }
}

// 6. 綁定事件
yearSelect.addEventListener("change", updateDays);
monthSelect.addEventListener("change", updateDays);
daySelect.addEventListener("change", updateBirthDate);

// 7. 預設選項
yearSelect.selectedIndex = 0;
monthSelect.selectedIndex = 0;
updateDays();

window.renderAddNewButton = function (url) {
    let popupContent = $(".dx-popup-content")
    let footer = popupContent.find(".dx-popup-content-footer")

    popupContent.addClass("dx-popup-content-with-footer")

    if (footer.length)
        return false

    popupContent.append(`
        <div class="dx-popup-content-footer py-2 px-3 border-top">
            <a href="${url}" target="_blank">+ Add New</a>
        </div>
    `);
}
$(document).ready(function () {

    var token = $.cookie("Token");
    var areaName = "Report";
    var entityName = "ReportTemplate";
    var url = "/" + areaName + "/" + entityName + "/Renderer";

    $('#btnGenerateReport').on('click', function () {
        let record = $('#form-main').serializeToJSON();
        //console.log(record);

        let p = encodeURIComponent(JSON.stringify(record));
        let rendererUrl = url + "?p=" + p;
        window.location = rendererUrl;
    });
});
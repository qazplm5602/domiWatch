$(function() {
    $(".download-text").click(function() {
        location.href = "./download";
    });

    $.each($(".server-box"), function(_, element) {
        const endpoint = $(element).attr("domi-endpoint");
        const uri = $(element).attr("domi-uri");
        if (endpoint === undefined || uri === undefined) return;

        const StartTime = new Date();        
        $.ajax({
            url:"https://"+endpoint+"/"+uri,
            cache: false,
            success: function(count) {
                count = Number(count);
                $(element).find(".status").text("온라인").removeClass("offline").addClass("online");
                $($(element).find(".half-list").children()[1]).html(`Ping: ${new Date() - StartTime}ms <span>${count}명 접속중</span>`);
            },
            error: function() {
                $(element).find(".status").text("오프라인");
            }
        });
    });
});


amplify.request.decoders.appEnvelope =
    function (data, status, xhr, success, error) {
    	if (status == "success") {
    		success(data, status);
    	} else {
    		error($.parseJSON(xhr.responseText), status);
    	}
    };

amplify.request.define(
	"register", "ajax",
	{
		url: "api/users",
		type: "POST",
		dataType: "json",
		cache: false,
		decoder: "appEnvelope"
	}
);
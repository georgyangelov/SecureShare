

amplify.request.decoders.errorDecoder =
    function (data, status, xhr, success, error) {
    	if (status == "success") {
    		success(data, status);
    	} else {
    		error($.parseJSON(xhr.responseText), xhr.status);
    	}
    };

amplify.request.define(
	"register", "ajax",
	{
		url: "api/users",
		type: "POST",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
    "login", "ajax",
    {
        url: "api/users/login",
        type: "POST",
        dataType: "json",
        cache: false,
        decoder: "errorDecoder"
    }
);

amplify.request.define(
	"userInfo", "ajax",
	{
		url: "api/users/{userId}",
		type: "GET",
		datatype: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"userLoginInfo", "ajax",
	{
		url: "api/users/{userId}/{sessionKey}",
		type: "GET",
		datatype: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"logout", "ajax",
	{
		url: "api/users/logout/{sessionKey}",
		type: "DELETE",
		datatype: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
    "checkEmail", "ajax",
    {
        url: "api/users/checkEmail/{email}",
        type: "GET",
        dataType: "json",
        cache: false,
        decoder: "errorDecoder"
    }
);

amplify.request.define(
	"updateUser", "ajax",
	{
		url: "api/users/",
		type: "PUT",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
    "checkChannelName", "ajax",
    {
    	url: "api/channels/check/{channelName}",
    	type: "GET",
    	dataType: "json",
    	cache: false,
    	decoder: "errorDecoder"
    }
);

amplify.request.define(
	"registerChannel", "ajax",
	{
		url: "api/channels",
		type: "POST",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"updateChannel", "ajax",
	{
		url: "api/channels/{channelName}",
		type: "PUT",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"deleteChannel", "ajax",
	{
		url: "api/channels/{channelName}",
		type: "DELETE",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"getChannelInfo", "ajax",
	{
		url: "api/channels/{channelName}",
		type: "GET",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"subscribeToChannel", "ajax",
	{
		url: "api/channels/{channelName}/users",
		type: "POST",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"unsubscribeFromChannel", "ajax",
	{
		url: "api/channels/{channelName}/users/{userId}",
		type: "DELETE",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"uploadEntity", "ajax",
	{
		url: "api/channels/{channelName}/entities",
		type: "POST",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);

amplify.request.define(
	"getEntities", "ajax",
	{
		url: "api/channels/{channelName}/entities",
		type: "GET",
		dataType: "json",
		cache: false,
		decoder: "errorDecoder"
	}
);
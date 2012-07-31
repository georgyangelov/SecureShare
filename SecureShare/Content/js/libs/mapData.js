function mapArray(from, to, keyFunc, createFunc, append) {

	if (!append) {
		var temp = to;
		for (var i = 0; i < temp.length; i++) {
			if (!arrayContains(from, keyFunc, keyFunc(to[i]))) {
				to.splice(i, 1);
			}
		}
	}

	for (var i = 0; i < from.length; i++) {
		if (!arrayContains(to(), keyFunc, keyFunc(from[i]))) {
			to.push(createFunc(from[i]));
		}
	}

	return to;
}

function arrayContains(array, keyFunc, key) {
	for (var i = 0; i < array.length; i++) {
		if (keyFunc(array[i]) == key)
			return true;
	}

	return false;
}
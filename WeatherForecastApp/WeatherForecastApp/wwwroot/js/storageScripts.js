var weatherScript = weatherScript || {};

weatherScript.getStorage = () => {
    const existingData = JSON.parse(localStorage.getItem('oldCity')) || [];
    return existingData;
}

weatherScript.getNotificationStorage = () => {
    const existingTime = JSON.parse(localStorage.getItem('timeToShow')) || [];
    return existingTime;
}

weatherScript.setStorage = (city) => {
    const existingData = JSON.parse(localStorage.getItem('newCity')) || [];
    const existingTime = JSON.parse(localStorage.getItem('timeToShow')) || [];

    let objectExists = false;
    let timeExists = false;

    if (existingData.length >= 1) {
        objectExists = existingData.some(obj => obj.Key === city.Key);
    }

    if (existingTime.length >= 1) {
        timeExists = existingTime.some(obj => obj.Key === city.Key);
    }

    if (!objectExists) {
        existingData.push(city);
        localStorage.setItem('newCity', JSON.stringify(existingData));
    }

    if (!timeExists && (city.PrecipitationTypeDay === "Rain" || city.PrecipitationTypeNight === "Rain")) {
        const currentTime = new Date();
        let dayWasShown = { Key: city.Key, LastTimeShown: currentTime };
        existingTime.push(dayWasShown);
        localStorage.setItem('timeToShow', JSON.stringify(existingTime));

        setTimeout(function () {
            alert("It will rain in " + city.LocalizedName);
        }, 2000)

        var now = new Date(currentTime);

        var midnight = new Date(currentTime);
        midnight.setHours(24, 0, 0, 0);

        var timeUntilMidnight = midnight - now;

        // Встановлюємо таймер на очищення localStorage за цей час
        setTimeout(function () {
            localStorage.clear();
            console.log("LocalStorage очищено о 00:00");
        }, timeUntilMidnight);
    }
}

weatherScript.updateOldStorage = () => {
    const existingData = JSON.parse(localStorage.getItem('newCity')) || [];
    localStorage.setItem('oldCity', JSON.stringify(existingData));
    localStorage.removeItem('newCity');
}
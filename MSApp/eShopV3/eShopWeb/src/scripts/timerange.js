const timerange = {
    getPreviousDay(date) {
        const previous = new Date(date.getTime());
        previous.setDate(date.getDate() - 1);

        return previous;
    },
    /**
     * Lấy khoảng thời gian theo ngày (từ đầu tới cuối ngày)
     * @param {date} date (Ngày)
     * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
     * Author: NVMANH (02/08/2019)
     */
    getRangeDateTimeInDay(date) {
        var startDateTime = date.setHours(0, 0, 0, 0);
        var endDateTime = date.setHours(23, 59, 59, 999);
        return {
            StartDateTime: new Date(startDateTime),
            EndDateTime: new Date(endDateTime)
        };
    },

    /**
     * Lấy khoảng thời gian theo ngày (từ đầu tới cuối ngày)
     * @param {date} date (Ngày)
     * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
     * Author: NVMANH (02/08/2019)
     */
    getRangeDateTimeInYesterday(date) {
        date = this.getPreviousDay(date);
        var startDateTime = date.setHours(0, 0, 0, 0);
        var endDateTime = date.setHours(23, 59, 59, 999);
        return {
            StartDateTime: new Date(startDateTime),
            EndDateTime: new Date(endDateTime)
        };
    },

    /**
     * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
     * @param {any} date (Ngày)
     * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
     * Author: NVMANH (02/08/2019)
     */
    getRangeDateTimeWeekOfDate(date) {
        // Lấy ngày đầu tuần (thứ 2):
        var first = date.getDate() - date.getDay() + 1; // First day is the day of the month - the day of the week

        var monday = new Date(date.setDate(first));
        var sunday = monday.addDays(6);

        return {
            StartDateTime: new Date(monday.setHours(0, 0, 0, 0)),
            EndDateTime: new Date(sunday.setHours(23, 59, 59, 999))
        };
    },

    /**
     * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
     * @param {any} date (Ngày)
     * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
     * Author: NVMANH (02/08/2019)
     */
    getRangeDateTimeMonthOfDate(date) {
        var startDateTime = new Date(date.setDate(1));
        var endDateTime = new Date(date.getFullYear(), date.getMonth() + 1, 0);
        return {
            StartDateTime: new Date(startDateTime.setHours(0, 0, 0, 0)),
            EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999))
        };
    },

    /**
     * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
     * @param {any} date (Ngày)
     * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
     * Author: NVMANH (02/08/2019)
     */
    getRangeDateTimeQuarterOfDate(date) {
        var thisQuater = Math.ceil((date.getMonth() + 1) / 3);
        var startMonth = thisQuater * 3 - 3;
        var endMonth = thisQuater * 3 - 1;
        var startDateTime = new Date(date.getFullYear(), startMonth, 1);
        var endDateTime = new Date(date.getFullYear(), endMonth + 1, 0);
        return {
            StartDateTime: new Date(startDateTime.setHours(0, 0, 0, 0)),
            EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999))
        };
    },
    /**
     * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
     * @param {any} date (Ngày)
     * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
     * Author: NVMANH (02/08/2019)
     */
    getRangeDateTimeYearOfDate(date) {
        var currentYear = date.getFullYear();
        var startDateTime = new Date(currentYear, 0, 1);
        var endDateTime = new Date(currentYear, 12, 0);
        return {
            StartDateTime: new Date(startDateTime.setHours(0, 0, 0, 0)),
            EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999))
        };
    }
}

export default timerange;
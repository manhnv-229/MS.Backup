const timeUtility = {
  Interval: [],
  /** ---------------------------------------------------------------------------
   * Set thời gian được tính toán từ 1 khoảng thời gian đến thời điểm hiện tại:
   * @param {any} timeStart thời gian bắt đầu tính toán
   * @param {any} timeLoop  thời gian sẽ lặp lại việc tính toán (có thể không truyền)
   * @param {any} callback  phương thức callback sau khi thực hiện tính toán được
   * Author: NVMANH (19/07/2019)
   */
  setCalculatorTime(timeStart, timeLoop, callback) {
    // Set the date we're counting down to
    var countDownDate = timeStart; //new Date("Wed Jun 19 2019 16:58:17").getTime();
    if (timeLoop) {
      // Update the count down every 1 second
      var x = window.setInterval(function () {
        const timeInfo = timeUtility.getTimeDistance(timeStart);
        var days = timeInfo.Days;
        var hours = timeInfo.Hours;
        var minutes = timeInfo.Minutes;
        var seconds = timeInfo.Seconds;
        var hoursText = hours < 10 ? "0" + hours : hours;
        var minutesText = minutes < 10 ? "0" + minutes : minutes;
        var dayText = days > 0 ? days + "d " : "";
        seconds = seconds < 10 ? "0" + seconds : seconds;

        // Display the result in the element with id="demo"
        if (callback) {
          const timeInfoString = `${
            dayText || ""
          } ${hoursText}:${minutesText}:${seconds}`;
          callback(timeInfo, timeInfoString, x);
        }
      }, timeLoop);
      timeUtility.Interval.push(x);
    }
  },
  stopTimeCalculator(intervalId){
    const index = timeUtility.Interval.indexOf(intervalId);
    if (index>-1) {
      timeUtility.Interval.splice(index,1);
    }
    clearInterval(intervalId);
  },
  /** ---------------------------------------------------------------------------
   * Hàm thực hình tính toán khoảng cách giữa 2 khoảng thời gian
   * @param {any} startTimne Thời gian bắt đầu
   * @param {any} endTime Thời gian kết thúc (Nếu không truyền tham số vào thì sẽ lấy ngày hiện tại)
   * Author: NVMANH (19/07/2019)
   */
  getTimeDistance(startTimne, endTime) {
    // Find the distance between now and the count down date
    if (!endTime) {
      endTime = new Date().getTime();
    }
    if (startTimne) {
      startTimne = new Date(startTimne);
    }

    var distance = endTime - startTimne.getTime();
    // Time calculations for days, hours, minutes and seconds
    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor(
      (distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)
    );
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);

    return {
      Days: days,
      Hours: hours,
      Minutes: minutes,
      Seconds: seconds,
    };
  },

  getHourDistance(startTimne, endTime){
    // Find the distance between now and the count down date
    if (!endTime) {
      endTime = new Date().getTime();
    }
    if (startTimne) {
      startTimne = new Date(startTimne);
    }

    var distance = endTime - startTimne.getTime();
    // Time calculations for days, hours, minutes and seconds
    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor(
      (distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)
    );
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);

    const totalHourDay = days * 24;
    const minuteHour = minutes/60;
    const secondHour = seconds/60/60;
    return totalHourDay + minuteHour + secondHour;
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
      EndDateTime: new Date(endDateTime),
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
      EndDateTime: new Date(sunday.setHours(23, 59, 59, 999)),
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
      EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999)),
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
      EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999)),
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
      EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999)),
    };
  },
};
export default timeUtility;

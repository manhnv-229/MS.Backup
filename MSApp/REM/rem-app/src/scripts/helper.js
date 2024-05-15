function calculatorMoneyByHour(price, hours) {
  let totalAmount = (hours || 0) * (price || 0);
  // Làm tròn tiền:
  if (totalAmount > 10000) {
    totalAmount = Math.round(totalAmount / 10000) * 10000;
  } else if (totalAmount >= 1000) {
    totalAmount = Math.round(totalAmount / 1000) * 1000;
  }
  return totalAmount;
}

function toggleFullScreen(elem) {
  // ## The below if statement seems to work better ## if ((document.fullScreenElement && document.fullScreenElement !== null) || (document.msfullscreenElement && document.msfullscreenElement !== null) || (!document.mozFullScreen && !document.webkitIsFullScreen)) {
  if (
    (document.fullScreenElement !== undefined &&
      document.fullScreenElement === null) ||
    (document.msFullscreenElement !== undefined &&
      document.msFullscreenElement === null) ||
    (document.mozFullScreen !== undefined && !document.mozFullScreen) ||
    (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)
  ) {
    if (elem.requestFullScreen) {
      elem.requestFullScreen();
    } else if (elem.mozRequestFullScreen) {
      elem.mozRequestFullScreen();
    } else if (elem.webkitRequestFullScreen) {
      elem.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
    } else if (elem.msRequestFullscreen) {
      elem.msRequestFullscreen();
    }
  } else {
    if (document.cancelFullScreen) {
      document.cancelFullScreen();
    } else if (document.mozCancelFullScreen) {
      document.mozCancelFullScreen();
    } else if (document.webkitCancelFullScreen) {
      document.webkitCancelFullScreen();
    } else if (document.msExitFullscreen) {
      document.msExitFullscreen();
    }
  }
}

function replaceDoubleQuotationMark(text) {
  try {
    if (text && typeof (text) === "string") {
      text = text.replaceAll('``', '"');
      return text;
    } else {
      return text;
    }
  } catch (error) {
    return text;
  }
}

export { calculatorMoneyByHour, toggleFullScreen, replaceDoubleQuotationMark };

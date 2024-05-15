import $ from "jquery";
var BarcodeScanerEvents = function () {
  this.initialize.apply(this, arguments);
};

BarcodeScanerEvents.prototype = {
  initialize: function () {
    $(document).on({
      keyup: $.proxy(this._keyup, this),
    });
  },
  _timeoutHandler: 0,
  _inputString: "",
  _keyup: function (e) {
    if (this._timeoutHandler) {
      clearTimeout(this._timeoutHandler);
      this._inputString += String.fromCharCode(e.which);
    }

    this._timeoutHandler = setTimeout(
      $.proxy(function () {
        if (this._inputString.length <= 3) {
          this._inputString = "";
          return;
        }
        const data = {
          target: e.target,
          input: this._inputString,
        };
        $(document).trigger("onbarcodescaned", data);

        this._inputString = "";
      }, this),
      20
    );
  },
};

const barcodeScanerEvent = new BarcodeScanerEvents();
export default barcodeScanerEvent;

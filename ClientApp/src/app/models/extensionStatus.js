"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ExtensionStatus = /** @class */ (function () {
    function ExtensionStatus(ID, Status, SubStatus, LastActivity, LastActivityIdle, ITime, ATime, CTime, TotalCalls, TotalICalls, TotalOCalls, LoggedInTime, IdleTimeList, StatusList, SubStatusList, DurationList, LogoutTimesList) {
        this.ID = ID;
        this.Status = Status;
        this.SubStatus = SubStatus;
        this.LastActivity = LastActivity;
        this.LastActivityIdle = LastActivityIdle;
        this.ITime = ITime;
        this.ATime = ATime;
        this.CTime = CTime;
        this.TotalCalls = TotalCalls;
        this.TotalICalls = TotalICalls;
        this.TotalOCalls = TotalOCalls;
        this.LoggedInTime = LoggedInTime;
        this.IdleTimeList = IdleTimeList;
        this.StatusList = StatusList;
        this.SubStatusList = SubStatusList;
        this.DurationList = DurationList;
        this.LogoutTimesList = LogoutTimesList;
    }
    return ExtensionStatus;
}());
exports.ExtensionStatus = ExtensionStatus;
//# sourceMappingURL=extensionStatus.js.map
mergeInto(LibraryManager.library, {
    unityRoomSendScore: function(score) {
        // unityroomのランキングAPIにスコアを送信
        if (typeof window.unityroom !== 'undefined' && typeof window.unityroom.API !== 'undefined') {
            window.unityroom.API.RecordScore(score);
            console.log('[UnityroomAPI.jslib] スコアを送信: ' + score);
        } else {
            console.warn('[UnityroomAPI.jslib] unityroom API が見つかりません（ローカル環境またはunityroom以外）');
        }
    }
});

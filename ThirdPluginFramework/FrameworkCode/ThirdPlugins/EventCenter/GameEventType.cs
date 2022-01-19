public enum GameEventType
{
    #region 系统事件
    INIT_NEW_PLAYER,//初始化一个新玩家
    DELAY_SHOW_INTER,//延迟展示插屏
    #endregion

    #region UI事件
    PRELOAD_ALL_UI,
    OPEN_UI_UNUSE_QUEUE,//不使用队列打开UI
    OPEN_UI_USE_QUEUE,//使用队列打开UI
    CHECK_QUEUE_AND_OPEN_NEXT_UI,//检查队列并打开下一个UI
    INIT_SETTLEMENT,//初始化结算界面
    INIT_ROOT_FINISH,
    REMOVE_LAST_MESSAGETIPS,
    MESSAGETIPS,
    UPDATE_PLAYER_COIN_SHOW,//更新玩家金钱显示
    CLOSE_GAME_UI,//关闭游戏界面
    ON_DRAG_AREA,//拖动区域
    INIT_ONE_SUB_ITEM,//初始化一个显示数量减的Item
    #endregion

    #region 皮肤相关
    SELECTING_ITEM_BY_ID,
    SELECTING_SKIN,
    SELECT_SKIN_TYPE,
    LOCK_ITEM_BY_ID,
    SHOW_RAND_TARGET_BY_ID,
    SHOW_RAND_EFFECT_BY_ID,
    UNLOCK_ITEM_BY_ID,
    UPDATE_PLAYER_SKIN,//更新玩家皮肤
    #endregion

    #region 游戏点击事件
    MOVE_CAR_BY_POINT,
    START_CAR_MOVE,
    STOP_CAR_MOVE,
    #endregion

    #region 游戏内事件
    INIT_LEVEL_BY_STAGE,//初始化关卡
    SET_CAMERA_FOLLOW_TARGET,//设置摄像机跟随目标
    REMOVE_CAMERA_FOLLOW_TARGET,//移除摄像机跟随目标
    AUTO_RESIZE,//自动归位
    UPDATE_ALIVE_COUNT,//更新玩家存活数量
    INIT_ONE_BULLET,//初始化一个子弹
    RECYCLE_ALL_BULLET,//回收所有子弹
    RECYCLE_ALL_ENEMY,//回收所有敌人
    RECYCLE_ALL_PLAYER,//回收所有玩家
    STOP_SLIDER_MOVE,//停止滑块移动
    RESET_ENEMY_TARGET,//重置敌人寻路目标
    START_GAME_BY_FIRST,//开始激活第一波敌人
    STOP_PLAYER_MOVE,//玩家停止移动
    CHANGE_WEAPON_BY_SET,//玩家更换武器
    CHANGE_SKIN_BY_SET,//玩家更换皮肤
    MOVE_TO_TANK,//移动至坦克中
    EXIT_TO_TANK,//从坦克中出来
    LOCK_SLIDER,//锁定滑动
    UNLOCK_SLIDER,//解锁滑动
    SHOOT_TANK_BULLET,//发射坦克导弹
    MOVE_SPAWN_ENEMY,//给定敌人数据和位置移动生成敌人
        #region 木头人用事件
            WOODENMAN_STOP_MOVE,//木头人模式停止移动
            WOODENMAN_CONTINUE_MOVE,//木头人模式继续移动
            START_WOODENMAN_ROUND,//开始木头人回合
            END_WOODENMAN_ROUND,//结束木头人回合
            ROBOT_IM_ROT_BACK,//通知机器人立刻回头
            ROBOT_ROT_BACK,//通知机器人按预设速度背身
            ROBOT_ROT_FORWARD,//通知机器人按预设速度转正身体
            START_SPEED_UP_MOVE,//开始加速
            END_SPEED_UP_MOVE,//结束加速
            FALL_DOWN_EMOJI,//下落滑稽表情包
            SET_ROBOT_EYE_TO_RED,
            SET_ROBOT_EYE_TO_GREEN,
        #endregion
    #endregion





    #region 兼容SDK事件
    StartLoadReward,
    StartLoadInter,
    StopLoadReward,
    StopLoadInter,
    StartLoadImageInter,
    StopLoadImageInter,
    #endregion
    
    
    RefreshCoinText,
    RefreshStarText,
    CloseMainUI,
    StartSpicalStage,
    ChangePlayerModel,
    ChangeGlovesModel,
    CheckEpicBtn,
    Follow,
    SpawnPlayerByCount,
    OpenVoice,
    CloseVoice,
    OpenVibrate,
    CloseVibrate,
    CloseTwelveOlderUI,
    OpenBox,
    SelectItem,
    CoinChange,
    RefreshPlayerGrowBtns,
    ChangePlayerCar,
    RefreshEpicSkinId,
    CloseStoreUI,
    OpenStoreUI,
}


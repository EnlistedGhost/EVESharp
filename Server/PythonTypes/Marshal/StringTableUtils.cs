namespace PythonTypes.Marshal
{
    /// <summary>
    /// Utils for using strings on the StringTable
    /// </summary>
    public static class StringTableUtils
    {
        public enum EntryList
        {
            _corpid,
            _locationid,
            age,
            Asteroid,
            authentication,
            ballID,
            beyonce,
            bloodlineID,
            capacity,
            categoryID,
            character,
            characterID,
            characterName,
            characterType,
            charID,
            chatx,
            clientID,
            config,
            contraband,
            corporationDateTime,
            corporationID,
            createDateTime,
            customInfo,
            description,
            divisionID,
            DoDestinyUpdate,
            dogmaIM,
            EVE_System,
            flag,
            foo_SlimItem,
            gangID,
            Gemini,
            gender,
            graphicID,
            groupID,
            header,
            idName,
            invbroker,
            itemID,
            items,
            jumps,
            line,
            lines,
            locationID,
            locationName,
            macho_CallReq,
            macho_CallRsp,
            macho_MachoAddress,
            macho_Notification,
            macho_SessionChangeNotification,
            modules,
            name,
            objectCaching,
            objectCaching_CachedObject,
            OnChatJoin,
            OnChatLeave,
            OnChatSpeak,
            OnGodmaShipEffect,
            OnItemChange,
            OnModuleAttributeChange,
            OnMultiEvent,
            orbitID,
            ownerID,
            ownerName,
            quantity,
            raceID,
            RowClass,
            securityStatus,
            Sentry_Gun,
            sessionchange,
            singleton,
            skillEffect,
            squadronID,
            typeID,
            used,
            userID,
            util_CachedObject,
            util_IndexRowset,
            util_Moniker,
            util_Row,
            util_Rowset,
            _multicastID,
            AddBalls,
            AttackHit3,
            AttackHit3R,
            AttackHit4R,
            DoDestinyUpdates,
            GetLocationsEx,
            InvalidateCachedObjects,
            JoinChannel,
            LSC,
            LaunchMissile,
            LeaveChannel,
            OIDplus,
            OIDminus,
            OnAggressionChange,
            OnCharGangChange,
            OnCharNoLongerInStation,
            OnCharNowInStation,
            OnDamageMessage,
            OnDamageStateChange,
            OnEffectHit,
            OnGangDamageStateChange,
            OnLSC,
            OnSpecialFX,
            OnTarget,
            RemoveBalls,
            SendMessage,
            SetMaxSpeed,
            SetSpeedFraction,
            TerminalExplosion,
            address,
            alert,
            allianceID,
            allianceid,
            bid,
            bookmark,
            bounty,
            channel,
            charid,
            constellationid,
            corpID,
            corpid,
            corprole,
            damage,
            duration,
            effects_Laser,
            gangid,
            gangrole,
            hqID,
            issued,
            jit,
            languageID,
            locationid,
            machoVersion,
            marketProxy,
            minVolume,
            orderID,
            price,
            range,
            regionID,
            regionid,
            role,
            rolesAtAll,
            rolesAtBase,
            rolesAtHQ,
            rolesAtOther,
            shipid,
            sn,
            solarSystemID,
            solarsystemid,
            solarsystemid2,
            source,
            splash,
            stationID,
            stationid,
            target,
            userType,
            userid,
            volEntered,
            volRemaining,
            weapon,
            agent_missionTemplatizedContent_BasicKillMission,
            agent_missionTemplatizedContent_ResearchKillMission,
            agent_missionTemplatizedContent_StorylineKillMission,
            agent_missionTemplatizedContent_GenericStorylineKillMission,
            agent_missionTemplatizedContent_BasicCourierMission,
            agent_missionTemplatizedContent_ResearchCourierMission,
            agent_missionTemplatizedContent_StorylineCourierMission,
            agent_missionTemplatizedContent_GenericStorylineCourierMission,
            agent_missionTemplatizedContent_BasicTradeMission,
            agent_missionTemplatizedContent_ResearchTradeMission,
            agent_missionTemplatizedContent_StorylineTradeMission,
            agent_missionTemplatizedContent_GenericStorylineTradeMission,
            agent_offerTemplatizedContent_BasicExchangeOffer,
            agent_offerTemplatizedContent_BasicExchangeOffer_ContrabandDemand,
            agent_offerTemplatizedContent_BasicExchangeOffer_Crafting,
            agent_LoyaltyPoints,
            agent_ResearchPoints,
            agent_Credits,
            agent_Item,
            agent_Entity,
            agent_Objective,
            agent_FetchObjective,
            agent_EncounterObjective,
            agent_DungeonObjective,
            agent_TransportObjective,
            agent_Reward,
            agent_TimeBonusReward,
            agent_MissionReferral,
            agent_Location,
            agent_StandardMissionDetails,
            agent_OfferDetails,
            agent_ResearchMissionDetails,
            agent_StorylineMissionDetails,
        }

        public static string[] Entries = new string[]
        {
            "*corpid",
            "*locationid",
            "age",
            "Asteroid",
            "authentication",
            "ballID",
            "beyonce",
            "bloodlineID",
            "capacity",
            "categoryID",
            "character",
            "characterID",
            "characterName",
            "characterType",
            "charID",
            "chatx",
            "clientID",
            "config",
            "contraband",
            "corporationDateTime",
            "corporationID",
            "createDateTime",
            "customInfo",
            "description",
            "divisionID",
            "DoDestinyUpdate",
            "dogmaIM",
            "EVE System",
            "flag",
            "foo.SlimItem",
            "gangID",
            "Gemini",
            "gender",
            "graphicID",
            "groupID",
            "header",
            "idName",
            "invbroker",
            "itemID",
            "items",
            "jumps",
            "line",
            "lines",
            "locationID",
            "locationName",
            "macho.CallReq",
            "macho.CallRsp",
            "macho.MachoAddress",
            "macho.Notification",
            "macho.SessionChangeNotification",
            "modules",
            "name",
            "objectCaching",
            "objectCaching.CachedObject",
            "OnChatJoin",
            "OnChatLeave",
            "OnChatSpeak",
            "OnGodmaShipEffect",
            "OnItemChange",
            "OnModuleAttributeChange",
            "OnMultiEvent",
            "orbitID",
            "ownerID",
            "ownerName",
            "quantity",
            "raceID",
            "RowClass",
            "securityStatus",
            "Sentry Gun",
            "sessionchange",
            "singleton",
            "skillEffect",
            "squadronID",
            "typeID",
            "used",
            "userID",
            "util.CachedObject",
            "util.IndexRowset",
            "util.Moniker",
            "util.Row",
            "util.Rowset",
            "*multicastID",
            "AddBalls",
            "AttackHit3",
            "AttackHit3R",
            "AttackHit4R",
            "DoDestinyUpdates",
            "GetLocationsEx",
            "InvalidateCachedObjects",
            "JoinChannel",
            "LSC",
            "LaunchMissile",
            "LeaveChannel",
            "OID+",
            "OID-",
            "OnAggressionChange",
            "OnCharGangChange",
            "OnCharNoLongerInStation",
            "OnCharNowInStation",
            "OnDamageMessage",
            "OnDamageStateChange",
            "OnEffectHit",
            "OnGangDamageStateChange",
            "OnLSC",
            "OnSpecialFX",
            "OnTarget",
            "RemoveBalls",
            "SendMessage",
            "SetMaxSpeed",
            "SetSpeedFraction",
            "TerminalExplosion",
            "address",
            "alert",
            "allianceID",
            "allianceid",
            "bid",
            "bookmark",
            "bounty",
            "channel",
            "charid",
            "constellationid",
            "corpID",
            "corpid",
            "corprole",
            "damage",
            "duration",
            "effects.Laser",
            "gangid",
            "gangrole",
            "hqID",
            "issued",
            "jit",
            "languageID",
            "locationid",
            "machoVersion",
            "marketProxy",
            "minVolume",
            "orderID",
            "price",
            "range",
            "regionID",
            "regionid",
            "role",
            "rolesAtAll",
            "rolesAtBase",
            "rolesAtHQ",
            "rolesAtOther",
            "shipid",
            "sn",
            "solarSystemID",
            "solarsystemid",
            "solarsystemid2",
            "source",
            "splash",
            "stationID",
            "stationid",
            "target",
            "userType",
            "userid",
            "volEntered",
            "volRemaining",
            "weapon",
            "agent.missionTemplatizedContent_BasicKillMission",
            "agent.missionTemplatizedContent_ResearchKillMission",
            "agent.missionTemplatizedContent_StorylineKillMission",
            "agent.missionTemplatizedContent_GenericStorylineKillMission",
            "agent.missionTemplatizedContent_BasicCourierMission",
            "agent.missionTemplatizedContent_ResearchCourierMission",
            "agent.missionTemplatizedContent_StorylineCourierMission",
            "agent.missionTemplatizedContent_GenericStorylineCourierMission",
            "agent.missionTemplatizedContent_BasicTradeMission",
            "agent.missionTemplatizedContent_ResearchTradeMission",
            "agent.missionTemplatizedContent_StorylineTradeMission",
            "agent.missionTemplatizedContent_GenericStorylineTradeMission",
            "agent.offerTemplatizedContent_BasicExchangeOffer",
            "agent.offerTemplatizedContent_BasicExchangeOffer_ContrabandDemand",
            "agent.offerTemplatizedContent_BasicExchangeOffer_Crafting",
            "agent.LoyaltyPoints",
            "agent.ResearchPoints",
            "agent.Credits",
            "agent.Item",
            "agent.Entity",
            "agent.Objective",
            "agent.FetchObjective",
            "agent.EncounterObjective",
            "agent.DungeonObjective",
            "agent.TransportObjective",
            "agent.Reward",
            "agent.TimeBonusReward",
            "agent.MissionReferral",
            "agent.Location",
            "agent.StandardMissionDetails",
            "agent.OfferDetails",
            "agent.ResearchMissionDetails",
            "agent.StorylineMissionDetails",
        };
    }
}
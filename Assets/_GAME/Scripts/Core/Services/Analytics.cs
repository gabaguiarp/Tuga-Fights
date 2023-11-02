using Firebase.Analytics;
using System.Text;
using UnityEngine;

namespace MemeFight.Services
{
    public class Analytics
    {
        public enum Event
        {
            LOGIN,
            OPEN_TUTORIAL,
            CAMPAIGN_START,
            CAMPAIGN_COMPLETE,
            CAMPAIGN_GIVE_UP,
            FREEFIGHT_MATCH_START,
            ROUND_WIN,
            ROUND_LOSS,
            MATCH_COMPLETE,
            PERFECT_WIN,
            QUESTLINE_COMPLETE,
            BATATOON_MATCH_STARTED,
            NOTIFICATION_TAPPED
        }

        // EVENT NAMES
        static readonly string EventOpenTutorial = "open_tutorial";
        static readonly string EventCampaignStart = "campaign_start";
        static readonly string EventCampaignComplete = "campaign_complete";
        static readonly string EventCampaignGiveUp = "campaign_give_up";
        static readonly string EventFreeFightMatchStart = "freefight_match_start";
        static readonly string EventRoundWin = "round_win";
        static readonly string EventRoundLoss = "round_loss";
        static readonly string EventMatchComplete = "match_complete";
        static readonly string EventPerfectWin = "perfect_win";
        static readonly string EventQuestlineComplete = "questline_complete";
        static readonly string EventBatatoonMatchStarted = "batatoon_match_started";
        static readonly string EventNotificationTapped = "app_open_through_notification";

        // PARAMETER NAMES
        static readonly string ParameterSelectedTeam = "selected_team";
        static readonly string ParameterBacalhau = "bacalhau";
        static readonly string ParameterAzeite = "azeite";
        static readonly string ParameterSelectedFighter = "selected_fighter";
        static readonly string ParameterOpponentAssigned = "opponent_assigned";
        static readonly string ParameterConsecutiveWins = "consecutive_wins";
        static readonly string ParameterConsecutiveLosses = "consecutive_losses";
        static readonly string ParameterRoundDuration = "round_duration";
        static readonly string ParameterActiveSessionMatchesPlayed = "active_session_matches_played";

        static string GetNameParameterForTeam(Team team)
        {
            return team.Equals(Team.Bacalhau) ? ParameterBacalhau : ParameterAzeite;
        }

        public static void RegisterEvent(Event evt, params string[] parameters)
        {
            switch (evt)
            {
                case Event.LOGIN:
                    LogEvent(FirebaseAnalytics.EventLogin);
                    break;

                case Event.OPEN_TUTORIAL:
                    LogEvent(EventOpenTutorial);
                    break;

                case Event.CAMPAIGN_START:
                    LogEvent(EventCampaignStart, ParameterSelectedTeam,
                             GetNameParameterForTeam(ResourcesManager.PersistentData.SelectedTeam));
                    break;

                case Event.CAMPAIGN_COMPLETE:
                    LogEvent(EventCampaignComplete, ParameterSelectedTeam,
                             GetNameParameterForTeam(ResourcesManager.PersistentData.SelectedTeam));
                    break;

                case Event.CAMPAIGN_GIVE_UP:
                    LogEvent(EventCampaignGiveUp);
                    break;

                case Event.FREEFIGHT_MATCH_START:
                    if (parameters == null || parameters.Length == 0)
                    {
                        LogParameterlessEventError(Event.FREEFIGHT_MATCH_START);
                        return;
                    }

                    LogEvent(EventFreeFightMatchStart, ParameterSelectedFighter, parameters[0]);
                    break;

                case Event.ROUND_WIN:
                    LogIntPairEvent(EventRoundWin, ParameterConsecutiveWins, ResourcesManager.PersistentData.ConsecutiveWins,
                                    ParameterRoundDuration, ResourcesManager.PersistentData.LatestRoundDurationRounded);
                    break;

                case Event.ROUND_LOSS:
                    if (parameters == null || parameters.Length == 0)
                    {
                        LogParameterlessEventError(Event.ROUND_LOSS);
                        return;
                    }

                    // This event logs 3 parameters:
                    // Consecutive losses | Latest round duration | Label of the player's opponent
                    LogIntStringPairEvent(EventRoundLoss,
                        new EventParameter<int>[2]
                        {
                            new EventParameter<int>(ParameterConsecutiveLosses, ResourcesManager.PersistentData.ConsecutiveLosses),
                            new EventParameter<int>(ParameterRoundDuration, ResourcesManager.PersistentData.LatestRoundDurationRounded)
                        },
                        new EventParameter<string>[1]
                        {
                            new EventParameter<string>(ParameterOpponentAssigned, parameters[0])
                        });
                    break;

                case Event.MATCH_COMPLETE:
                    LogEvent(EventMatchComplete, ParameterActiveSessionMatchesPlayed,
                             ResourcesManager.PersistentData.ActiveSessionMatchesPlayed);
                    break;

                case Event.PERFECT_WIN:
                    LogEvent(EventPerfectWin);
                    break;

                case Event.QUESTLINE_COMPLETE:
                    LogEvent(EventQuestlineComplete);
                    break;

                case Event.BATATOON_MATCH_STARTED:
                    LogEvent(EventBatatoonMatchStarted);
                    break;

                case Event.NOTIFICATION_TAPPED:
                    LogEvent(EventNotificationTapped);
                    break;
            }
        }

        #region Event Logging
        static void LogEvent(string name)
        {
            FirebaseAnalytics.LogEvent(name);
            LogMessageFormat("Event '{0}' successfully logged!", name);
        }

        static void LogEvent(string name, string parameterName, string parameterValue)
        {
            FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
            LogMessageFormat("Event '{0}', with parameter '{1}: {2}' successfully logged!", name, parameterName, parameterValue);
        }

        static void LogEvent(string name, string parameterName, int parameterValue)
        {
            FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
            LogMessageFormat("Event '{0}', with parameter '{1}: {2}' successfully logged!", name, parameterName, parameterValue);
        }

        static void LogIntPairEvent(string eventName, string param1Name, int param1Value, string param2Name, int param2Value)
        {
            Parameter[] intPairParameters =
            {
                new Parameter(param1Name, param1Value),
                new Parameter(param2Name, param2Value)
            };

            FirebaseAnalytics.LogEvent(eventName, intPairParameters);
            LogMessageFormat("Event '{0}', with parameters '{1}: {2}' and '{3}: {4}' successfully logged!", eventName, param1Name,
                             param1Value, param2Name, param2Value);
        }

        static void LogIntStringPairEvent(string eventName, EventParameter<int>[] intParams, EventParameter<string>[] stringParams)
        {
            Parameter[] parameters = new Parameter[intParams.Length + stringParams.Length];
            int paramIndex = 0;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Event '{0}', with parameters ", eventName);

            for (int i = 0; i < intParams.Length; i++)
            {
                parameters[paramIndex] = new Parameter(intParams[i].name, intParams[i].value);
                sb.AppendFormat("'{0}: {1}'", intParams[i].name, intParams[i].value);

                if (paramIndex < parameters.LastIndex())
                {
                    sb.Append(paramIndex < (parameters.LastIndex() - 1) ? ", " : " and ");
                    paramIndex++;
                }
            }

            for (int j = 0; j < stringParams.Length; j++)
            {
                parameters[paramIndex] = new Parameter(stringParams[j].name, stringParams[j].value);
                sb.AppendFormat("'{0}: {1}'", stringParams[j].name, stringParams[j].value);

                if (paramIndex < parameters.LastIndex())
                {
                    sb.Append(paramIndex < (parameters.LastIndex() - 1) ? ", " : " and ");
                    paramIndex++;
                }
            }

            sb.Append(" successfully logged!");

            FirebaseAnalytics.LogEvent(eventName, parameters);
            LogMessage(sb.ToString());
        }
        #endregion

        #region Debug Messages
        static void LogMessage(string message)
        {
            Debug.Log("[Firebase] " + message);
        }

        static void LogMessageFormat(string message, params object[] args)
        {
            LogMessage(string.Format(message, args));
        }

        static void LogParameterlessEventError(Event evt)
        {
            Debug.LogErrorFormat("[Firebase] Unable to log event '{0}' because no parameters were passed!", evt);
        }
        #endregion
    }

    class EventParameter<T>
    {
        public EventParameter(string name, T value)
        {
            this.name = name;
            this.value = value;
        }

        public string name;
        public T value;
    }
}

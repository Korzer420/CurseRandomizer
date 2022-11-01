using HutongGames.PlayMaker;
using System;

namespace CurseRandomizer.Helper;

public static class FsmHelper
{
    /// <summary>
    /// Check if the passed action is in the correct context for modifications. Leave unneeded values null.
    /// </summary>
    /// <param name="fsmName">The fsm in which the action should be.</param>
    /// <param name="gameObjectName">The name of the gameobject on which the fsm of this action is attached.</param>
    /// <param name="stateName">The name of the state in which this action should be.</param>
    /// <param name="action">The action for the comparison.</param>
    /// <returns></returns>
    public static bool IsCorrectContext(this FsmStateAction action, string fsmName, string gameObjectName, string stateName)
    {
        if (!string.IsNullOrEmpty(fsmName) && !string.Equals(fsmName, action.Fsm.Name, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrEmpty(gameObjectName) && !string.Equals(gameObjectName, action.Fsm.GameObjectName, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrEmpty(stateName) && !string.Equals(stateName, action.State.Name, StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }
}

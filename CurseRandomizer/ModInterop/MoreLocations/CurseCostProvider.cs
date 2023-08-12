using MoreLocations.Rando.Costs;
using RandomizerCore.Logic;
using System;

namespace CurseRandomizer.ModInterop.MoreLocations;

internal class CurseCostProvider : ICostProvider
{
    public bool HasNonFreeCostsAvailable => true;

    public LogicCost Next(LogicManager lm, Random rng) => new SimpleCost(lm.GetTerm("TAKECURSE"), rng.Next(1,4));
    
    public void PreRandomize(Random rng) { }
}

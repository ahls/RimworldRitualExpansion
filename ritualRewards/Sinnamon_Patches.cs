using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace Sinnamon_Ritual
{
    [StaticConstructorOnStartup]
    internal static class PatchDriver
    {
        static PatchDriver()
        {
            var harmony = new Harmony("Sinnamon.RitualReward");
            harmony.PatchAll();
        }
    }
    [HarmonyPatch(typeof(RitualAttachableOutcomeEffectDef), "CanAttachToRitual")]
    static class Patch
    {
        static void Postfix(ref AcceptanceReport __result,ref RitualAttachableOutcomeEffectDef __instance, Precept_Ritual ritual)
        {
            if(__instance.HasModExtension<Sinnamon_RitualExtension>())
            {
                List<MemeDef> forbidList = __instance.GetModExtension<Sinnamon_RitualExtension>().forbiddenMemeAny;

                if (!forbidList.NullOrEmpty<MemeDef>() && ritual.ideo.memes.SharesElementWith(forbidList))
                    {
                        string forbidListInString = (from m in forbidList select m.label.ResolveTags()).ToCommaList(false, false);
                    __result = "Sinnamon_MemeConflicts".Translate() + forbidListInString;

                    }
            }
        }
    }
}

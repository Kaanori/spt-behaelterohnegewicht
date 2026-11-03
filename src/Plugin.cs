// ---------------------------------------------------------------
//  Sicherheitsbehaelter ohne Gewicht
//
//  Das Tragegewicht laeuft im Spiel ueber genau EINE Stelle:
//      Inventory.TotalWeight = () => Equipment.GetEquipmentWeight()
//  Dort wird ueber alle Ausruestungsplaetze summiert. Ein Postfix
//  zieht anschliessend den Inhalt des Sicherheitsbehaelters wieder
//  ab - der Behaelter selbst wiegt weiter.
//
//  Warum "TotalWeight - Weight" und keine eigene Summe ueber den
//  Inhalt: so wird genau der Betrag abgezogen, den das Spiel vorher
//  aufaddiert hat. Rechnet das Spiel den Inhalt gar nicht mit, ist
//  die Differenz null und es passiert nichts - eine doppelte
//  Abrechnung ist damit ausgeschlossen.
// ---------------------------------------------------------------
using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT.InventoryLogic;
using HarmonyLib;

namespace BehaelterOhneGewicht
{
	[BepInPlugin("com.kaan.behaelterohnegewicht", "Sicherheitsbehaelter ohne Gewicht", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		internal static ManualLogSource Log;

		internal static ConfigEntry<bool> An;

		internal static ConfigEntry<bool> Protokoll;

		private void Awake()
		{
			Log = base.Logger;
			An = base.Config.Bind("1. Allgemein", "Eingeschaltet", defaultValue: true,
				"Der Inhalt des Sicherheitsbehaelters zaehlt nicht mehr zum Tragegewicht. "
				+ "Der Behaelter selbst wiegt weiter.");
			Protokoll = base.Config.Bind("9. Debug", "Abzug ins Protokoll schreiben", defaultValue: false,
				"Schreibt einmal je Aenderung mit, wie viel abgezogen wurde.");
			new Harmony("com.kaan.behaelterohnegewicht").PatchAll();
			Log.LogInfo("Sicherheitsbehaelter ohne Gewicht geladen.");
		}
	}

	internal static class Abzug
	{
		private static float _zuletzt = -1f;

		/// <summary>
		/// Was das Spiel fuer den Inhalt des Sicherheitsbehaelters
		/// angesetzt hat.
		/// </summary>
		public static float Inhalt(InventoryEquipment ausruestung)
		{
			try
			{
				if (ausruestung == null)
				{
					return 0f;
				}
				Slot fach = ausruestung.GetSlot(EquipmentSlot.SecuredContainer);
				Item behaelter = fach?.ContainedItem;
				if (behaelter == null)
				{
					return 0f;
				}
				float gesamt = behaelter.TotalWeight;
				float eigen = behaelter.Weight;
				float inhalt = gesamt - eigen;
				if (inhalt <= 0f || float.IsNaN(inhalt) || float.IsInfinity(inhalt))
				{
					return 0f;
				}
				if (Plugin.Protokoll.Value && Math.Abs(inhalt - _zuletzt) > 0.01f)
				{
					_zuletzt = inhalt;
					Plugin.Log.LogInfo($"Abzug: {inhalt:0.00} kg Inhalt "
						+ $"(Behaelter gesamt {gesamt:0.00}, eigen {eigen:0.00})");
				}
				return inhalt;
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Abzug: " + ex.Message);
				return 0f;
			}
		}
	}

	[HarmonyPatch(typeof(InventoryEquipment), "GetEquipmentWeight")]
	internal static class GewichtPatch
	{
		[HarmonyPostfix]
		private static void Danach(InventoryEquipment __instance, ref float __result)
		{
			if (!Plugin.An.Value)
			{
				return;
			}
			__result = Math.Max(0f, __result - Abzug.Inhalt(__instance));
		}
	}

	// Der zweite Wert wird fuer den Ausdauer-Bonus der Kraft-Fertigkeit
	// benutzt. Bliebe er unveraendert, waere das Gewicht an einer Stelle
	// abgezogen und an der anderen nicht.
	[HarmonyPatch(typeof(InventoryEquipment), "GetEquipmentWeightEliteSkill")]
	internal static class GewichtElitePatch
	{
		[HarmonyPostfix]
		private static void Danach(InventoryEquipment __instance, ref float __result)
		{
			if (!Plugin.An.Value)
			{
				return;
			}
			__result = Math.Max(0f, __result - Abzug.Inhalt(__instance));
		}
	}
}

﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Talon
{
    public static class DamageIndicator
    {
        private const int BarWidth = 106;
        private const float LineThickness = 9.8f;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private static DamageToUnitDelegate DamageToUnit { get; set; }

        private static Font _Font;

        public static void Initialize(DamageToUnitDelegate damageToUnit)
        {
            DamageToUnit = damageToUnit;
            Drawing.OnEndScene += OnEndScene;

            _Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 18,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Program.miscMenu["damageIndicatorDraw"].Cast<CheckBox>().CurrentValue/* || Settings.DrawPercent*/)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage = DamageToUnit(unit);

                    if (damage <= 0)
                    {
                        continue;
                    }

                    if (Program.miscMenu["damageIndicatorDraw"].Cast<CheckBox>().CurrentValue)
                    {
                        var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                               (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                        var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                        var startPoint = new Vector2((int)(unit.HPBarPosition.X + damagePercentage * BarWidth), (int)unit.HPBarPosition.Y - 5);
                        var endPoint = new Vector2((int)(unit.HPBarPosition.X + currentHealthPercentage * BarWidth) + 1, (int)unit.HPBarPosition.Y - 5);

                        //var colorH = System.Drawing.Color.FromArgb(127, 255, 212);

                        Drawing.DrawLine(startPoint, endPoint, LineThickness, System.Drawing.Color.Aquamarine);
                    }

                    /*if (Settings.DrawPercent)
                    {
                        var color = new Color(Settings.HealthColor.R, Settings.HealthColor.G, Settings.HealthColor.B, Settings.HealthColor.A - 5);
                        _Font.DrawText(null, string.Concat(Math.Ceiling(damage / unit.TotalShieldHealth() * 100), "%"), (int)unit.HPBarPosition[0] + 102, (int)unit.HPBarPosition[1] + 29, color);
                    }*/
                }
            }
        }
    }
}

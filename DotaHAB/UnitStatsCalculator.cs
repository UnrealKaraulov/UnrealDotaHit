using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Abilities;
using DotaHIT.DatabaseModel.Upgrades;
using DotaHIT.Core;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using Microsoft.Win32;
using DotaHIT.Core.Resources.Media;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;
using DotaHIT.MpqPath;
using DotaHIT.DatabaseModel.Format;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Extras;
using BitmapUtils;
using System.Drawing.Imaging;

namespace DotaHIT
{
    public enum DamageCalcType
    {
        None=0,
        Default=0,
        Stats=1,
        DPA=2
    }

    [Flags]
    public enum ArmorCalcType
    {
        None = 0,
        Default = 0,
        Evasion = 1,
        DamageSpecified = 2,        
        SurvivabilityMeasure = 4,
    }

    public class UnitStatsCalculator
    {
        // attack
        public StackedAbilitiesCollection attackStacked = new StackedAbilitiesCollection();
        public StackedAbilityResults onAttackStats = new StackedAbilityResults();
        // defense
        public StackedAbilitiesCollection defenseStacked = new StackedAbilitiesCollection();
        public StackedAbilityResults onNormalDefenseStats = new StackedAbilityResults();
        // enemy
        StackedAbilityResults onEnemyStats = new StackedAbilityResults();
        public DBABILITIES onEnemyAbilities = new DBABILITIES();   

        public readonly unit unit = null;
        int armorDamage = 0;

        public Damage damage;
        public DBDOUBLE bonus_damage;
        public Damage total_damage;
        public Damage additional_damage;
        public DBDOUBLE spell_damage;

        public DBDOUBLE hpFactor;

        public double total_ehp_normal;
        public double ehp_increase_normal;
        public double ehp_increase_normal_items;
        public double damage_reduction_normal;        

        public double total_ehp_spell;        
        public double ehp_increase_spell;
        public double ehp_increase_spell_items;
        public double spell_resistance;

        public double dps_spell;
        public double dps_normal;

        public UnitStatsCalculator(unit unit, int armorDamage)
        {
            this.unit = unit;
            this.armorDamage = armorDamage;
        }

        internal void PrepareInfo()
        {
            /////////////////////////
            //  PRE-CALC STATISTICS
            //////////////////////////

            ///////////////
            // on attack //
            ///////////////           

            attackStacked = StackedAbilities.GetPossibleCombinations<FreeStackedAbilities>(unit.onAttackAbilities);
            onAttackStats = new StackedAbilityResults(attackStacked);

            ////////////////
            // on defense //
            ////////////////

            defenseStacked = StackedAbilities.GetPossibleCombinations<FreeStackedAbilities>(unit.onDefenceAbilities);

            DBDAMAGE normalDamage = new DBDAMAGE(armorDamage, AttackType.Hero, DamageType.Normal);
            AbilityResults initial_results = new AbilityResults();
            initial_results.SetDbDamage("DAMAGE", normalDamage);

            onNormalDefenseStats = new StackedAbilityResults(defenseStacked, initial_results);

            //////////////
            // on enemy //
            //////////////

            onEnemyAbilities = unit.acquiredAbilities.GetSpecific(AbilitySpecs.Any,
                                            AbilityTriggerType.Always,
                                            AbilityMatchType.NotIntersects,
                                            TargetType.None, TargetType.Self);
            onEnemyStats = new StackedAbilityResults(onEnemyAbilities);
        }

        public void GetArmor(out double naked, out double bonus)
        {
            naked = unit.get_naked_armor();
            bonus = unit.armor - naked;
        }

        public void CalculateDamage(DamageCalcType damageCalcType)
        {
            spell_damage = 0;           

            switch (damageCalcType)
            {
                case DamageCalcType.Default:
                    damage = unit.get_naked_damage();
                    bonus_damage = ((int)unit.statsDamage - (int)damage) + unit.bonus_damage;
                    break;

                case DamageCalcType.Stats:
                    damage = unit.statsDamage;
                    bonus_damage = (int)unit.bonus_damage;
                    break;

                case DamageCalcType.DPA:
                    damage = unit.damage;
                    bonus_damage = (double)onAttackStats.GetDamage("ATTACK_DAMAGE"); // get full damage from crits or smth like that ...
                    if (bonus_damage != 0) bonus_damage -= onAttackStats.GetKeyProbability("ATTACK_DAMAGE") * damage; // bonus damage = full_damage - base_damage
                    bonus_damage += (double)onAttackStats.GetDamage("ATTACK_BONUS_DAMAGE"); // some other damage bonuses (from range bash)                        

                    /////////////////
                    // spell damage
                    /////////////////                        

                    spell_damage += (double)onAttackStats.GetDamage("DAMAGE_SPELL");
                    spell_damage += onEnemyStats.GetDouble("DPS_SPELL") * unit.cooldown;                    
                    break;

                default:
                    damage = 0;
                    bonus_damage = 0;
                    break;
            }

            //currentHeroDamage = damage;
            //currentHeroBonusDamage = bonus_damage;
            total_damage = damage + bonus_damage;
            additional_damage = total_damage - (Damage)unit.damage;
        }

        public void CalculateGeneralInfo(ArmorCalcType armorCalcType)
        {
            CalculateGeneralInfo(true, true, armorCalcType);
        }
        public void CalculateGeneralInfo(bool calculateEhp, bool calculateDps, ArmorCalcType armorCalcType)
        {
            if (calculateEhp)
            {
                hpFactor = 1.0 / (1.0 + Current.unit.hpFactor);

                /////////////////////////////
                // normal damage reduction
                /////////////////////////////

                double original_damage = (double)armorDamage;

                // calculating average damage received after it was blocked by Shield Block abilities
                DBDAMAGE reduced_damage = onNormalDefenseStats.GetDbDamage("DAMAGE");

                // use original damage for cases that do not trigger damage reducing abilities                
                reduced_damage += original_damage * (1 - onNormalDefenseStats.GetKeyProbability("DAMAGE"));

                int bonus_ehp = (int)(unit.armor * (unit.max_hp * 0.06));

                if (original_damage != reduced_damage)
                {
                    // total ehp                    

                    total_ehp_normal = ((unit.max_hp + bonus_ehp) * hpFactor) * (original_damage / reduced_damage);
                    total_ehp_normal = Math.Round(total_ehp_normal);
                    ehp_increase_normal = total_ehp_normal - (unit.max_hp * hpFactor);

                    // ehp bonuses from items

                    bonus_ehp = (int)(unit.get_naked_armor() * (unit.max_hp * 0.06));
                    double total_ehp_naked = ((unit.max_hp + bonus_ehp) * hpFactor);
                    ehp_increase_normal_items = total_ehp_normal - total_ehp_naked;

                    // damage reduction

                    damage_reduction_normal = ((0.06 * unit.armor) / (1 + (0.06 * unit.armor))) * hpFactor;
                    damage_reduction_normal = (1 - (reduced_damage / original_damage) * (1 - damage_reduction_normal)) * 100;
                }
                else
                {
                    // total ehp                    

                    total_ehp_normal = ((unit.max_hp + bonus_ehp) * hpFactor);
                    ehp_increase_normal = bonus_ehp * hpFactor;

                    // ehp bonuses from items

                    ehp_increase_normal_items = (int)((unit.armor - unit.get_naked_armor()) * (unit.max_hp * 0.06));

                    // damage reduction

                    damage_reduction_normal = ((6 * unit.armor) / (1 + (0.06 * unit.armor))) * hpFactor;
                }

                // recalculate ehp considering opponent's damage
                // if required "Armor:" switch is set
                // Formula: EHP = damage * ceiling(HP/damage)

                if ((armorCalcType & ArmorCalcType.SurvivabilityMeasure) != 0)
                    total_ehp_normal = original_damage * Math.Ceiling(total_ehp_normal / original_damage);

                // calculate evasion-to-ehp if required "Armor:" switch is set

                if ((armorCalcType & ArmorCalcType.Evasion) != 0)
                {
                    DBEVASIONABILITY evasion = unit.onDefenceAbilities.GetByType<DBEVASIONABILITY>();
                    if (evasion != null && evasion.Chance != 0)
                        total_ehp_normal = Math.Round(total_ehp_normal / (1 - evasion.Chance));
                }

                ///////////////////////////
                // spell damage reduction
                ///////////////////////////

                DBDAMAGE spellDamage = new DBDAMAGE(100, AttackType.Spell, DamageType.Magic);
                AbilityResults ar = new AbilityResults();
                ar.SetDbDamage("DAMAGE", spellDamage);
                foreach (DBABILITY a in unit.onDefenceAbilities)
                    a.Apply(ar);
                ar.Rename("RESISTED_DAMAGE", "DAMAGE");
                double resisted_damage = (double)(ar.GetDbDamage("DAMAGE").convert_to_double() * (1.0 - unit.spell_resistance));
                spell_resistance = 100 - resisted_damage;

                ehp_increase_spell = (int)(unit.max_hp * (spell_resistance / resisted_damage));
                total_ehp_spell = (unit.max_hp + ehp_increase_spell) * hpFactor;

                double baseSpellResistance = (double)unit.baseSpellResistance;
                ehp_increase_spell_items = ehp_increase_spell - (int)((double)((double)baseSpellResistance / (1.0 - baseSpellResistance)) * (double)(int)unit.max_hp);

                spell_resistance *= hpFactor;
                ehp_increase_spell *= hpFactor;
                ehp_increase_spell_items *= hpFactor;
            }

            if (calculateDps)
            {
                ////////////////////////////////
                // normal dps
                ////////////////////////////////

                dps_normal = unit.get_dps(this.additional_damage);

                ////////////////////////////////
                // spell dps
                ////////////////////////////////

                dps_spell = (double)onAttackStats.GetDamage("DAMAGE_SPELL") / unit.cooldown;
                dps_spell += onEnemyStats.GetDouble("DPS_SPELL");
            }
        }
    }
}

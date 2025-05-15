using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class Calc : SLStruct
{
    [SerializeField] SLValue character, weapon, other, trans;
    public SLValue main, maindef, sub, subdef;
    [SerializeField] SLValue skill;
    [SerializeField] Mod_InputField len;
    [SerializeField] Result grad, cur;
    public string json;

    public enum Type
    {
        百分比生命值, 百分比攻击力, 百分比防御力, 元素精通, 速度, 暴击率, 暴击伤害, 充能效率, 击破特攻, 伤害加成, 治疗加成, 护盾强效, 元素抗性, 伤害减免, 无视防御, 无视抗性, 敌方易伤, 剧变加成, 增幅加成, 基础生命值, 基础攻击力, 基础防御力, 数值生命值, 数值攻击力, 数值防御力, 总生命值, 总攻击力, 总防御力, len
    }
    public enum RateType
    {
        生命值倍率, 攻击力倍率, 防御力倍率, 元素精通倍率, 速度倍率, 击破倍率, 固定数值, len
    }
    public struct Status
    {
        public double[] values;
        public double[] entry_values;
        public double[] trans_values;//转化后属性
        public List<CalcTrans> trans;
        public void ParseBuff(List<Buff> buffs)
        {
            var dict = ValueDict.status_types;
            foreach (var buff in buffs)
            {
                if (buff.type.Length > 0)
                    values[dict[buff.type]] += buff.type.LastIndexOf("%") > 0 ? buff.value / 100 : buff.value;
            }
            if (Options.mode == Options.原神)
            {
                values[(int)Type.速度] = 0;
                values[(int)Type.击破特攻] = 0;
            }
            else if (Options.mode == Options.星穹铁道)
            {
                values[(int)Type.元素精通] = 0;
            }
        }
        public void ParseEntry(Entry entry)
        {
            for (int i = 0; i < entry.词条属性.Length; i++)
            {
                if (i == (int)Type.元素精通 || i == (int)Type.速度)
                    values[i] += entry.词条属性[i];
                else
                    values[i] += entry.词条属性[i] / 100;
            }
            if (Options.mode == Options.原神)
            {
                values[(int)Type.速度] = 0;
                values[(int)Type.击破特攻] = 0;
            }
            else if (Options.mode == Options.星穹铁道)
            {
                values[(int)Type.元素精通] = 0;
            }
        }
        public List<CalcTrans> ParseTrans(List<TransBuff> tr)
        {
            return ParseTrans(tr, trans);
        }
        public List<CalcTrans> ParseTrans(List<TransBuff> tr, List<CalcTrans> origin)
        {
            var src = origin ?? new List<CalcTrans>();
            var dict = ValueDict.status_types;
            foreach (var t in tr)
            {
                if (t.src.Length == 0 || t.dest.Length == 0)
                {
                    continue;
                }
                var p = t.src.LastIndexOf("%") > 0 ? 100 : 1;
                var p2 = t.dest.LastIndexOf("%") > 0 ? 100 : 1;
                var item = new CalcTrans() { src = dict[t.src], dest = dict[t.dest], value = t.rate / 100 * p / p2, start = t.start / p, end = t.end / p };
                if (item.src >= (int)Type.数值生命值 && item.src <= (int)Type.数值防御力)
                {
                    item.src += Type.总生命值 - Type.数值生命值;
                }
                src.Add(item);
            }
            return src;
        }

        public void CalcEntry(double[] entry = null)
        {
            Array.Copy(values, entry_values, (int)Type.len);
            if (entry != null)
            {
                for (int i = 0; i < (int)Type.护盾强效; i++)
                {
                    entry_values[i] += entry[i];
                }
            }
            CalcTotal(entry_values);
            if (Options.mode == Options.原神)
            {
                entry_values[(int)Type.速度] = 0;
                entry_values[(int)Type.击破特攻] = 0;
            }
            else if (Options.mode == Options.星穹铁道)
            {
                entry_values[(int)Type.元素精通] = 0;
            }
        }
        public void CalcTrans()
        {
            Array.Copy(entry_values, trans_values, (int)Type.len);
            foreach (var tr in trans)
            {
                double src = entry_values[tr.src] - tr.start;
                if (src > 0)
                {
                    if (src > tr.end && tr.end > tr.start)
                    {
                        src = tr.end;
                    }
                    trans_values[tr.dest] += src * tr.value;
                }
            }
            CalcTotal(trans_values);
            if (Options.mode == Options.原神)
            {
                trans_values[(int)Type.速度] = 0;
                trans_values[(int)Type.击破特攻] = 0;
            }
            else if (Options.mode == Options.星穹铁道)
            {
                trans_values[(int)Type.元素精通] = 0;
            }
            //Debug.Log(String.Format("{0} {1} {2} {3} {4} {5}", trans_values[(int)Type.总生命值], trans_values[(int)Type.总攻击力], trans_values[(int)Type.总防御力], entry_values[(int)Type.总生命值], entry_values[(int)Type.总攻击力], entry_values[(int)Type.总防御力]));
        }
        public void CalcTotal(double[] values)
        {
            values[(int)Type.总生命值] = values[(int)Type.基础生命值] * (1 + values[(int)Type.百分比生命值]) + values[(int)Type.数值生命值];
            values[(int)Type.总攻击力] = values[(int)Type.基础攻击力] * (1 + values[(int)Type.百分比攻击力]) + values[(int)Type.数值攻击力];
            values[(int)Type.总防御力] = values[(int)Type.基础防御力] * (1 + values[(int)Type.百分比防御力]) + values[(int)Type.数值防御力];
        }

        public Status(double hp, double atk, double def)
        {
            values = new double[(int)Type.len];
            trans_values = new double[(int)Type.len];
            entry_values = new double[(int)Type.len];
            values[(int)Type.基础生命值] = hp;
            values[(int)Type.基础攻击力] = atk;
            values[(int)Type.基础防御力] = def;
            trans = new List<CalcTrans>();
        }
        public Status(Status status)
        {
            values = new double[(int)Type.len];
            trans_values = new double[(int)Type.len];
            entry_values = new double[(int)Type.len];
            trans = new List<CalcTrans>(status.trans);
            Array.Copy(status.values, values, (int)Type.len);
            Array.Copy(status.trans_values, trans_values, (int)Type.len);
            Array.Copy(status.entry_values, entry_values, (int)Type.len);
        }
    }
    public struct Entry
    {
        public double[] 手动词条数;
        public double[] 词条数;
        public double[] 单词条收益;
        public double[] 词条属性;
        public double 自动分配词条;
        public Entry(List<Buff> buffs)
        {
            var dict = ValueDict.status_types;
            var dict2 = ValueDict.sub_values;
            手动词条数 = new double[(int)Type.护盾强效];
            词条数 = new double[(int)Type.护盾强效];
            单词条收益 = new double[(int)Type.护盾强效];
            词条属性 = new double[(int)Type.护盾强效];
            自动分配词条 = 0;
            foreach (var item in dict2)
            {
                单词条收益[dict[item.Key]] = item.Value / (item.Key.LastIndexOf("%") > 0 ? 100 : 1);
            }
            foreach (var buff in buffs)
            {
                if (buff.type.Length == 0)
                    continue;
                if (buff.type == "自动分配词条")
                    自动分配词条 += buff.value;
                else
                {
                    手动词条数[dict[buff.type]] += buff.value;
                    词条属性[dict[buff.type]] += buff.value * dict2[buff.type];
                }
            }
        }
        public void CalcValues(int end = (int)Type.护盾强效)
        {
            for (int i = 0; i < end; i++)
            {
                词条属性[i] = 词条数[i] * 单词条收益[i];
            }
        }
        public void AverageCnts()
        {
            for (int i = 0; i < (int)Type.伤害加成; i++)
            {
                词条数[i] = 自动分配词条 / (int)Type.伤害加成;
            }
        }
        public double[] TotalCnts()
        {
            double[] cnts = new double[(int)Type.护盾强效];
            for (int i = 0; i < (int)Type.护盾强效; i++)
            {
                cnts[i] = 手动词条数[i] + 词条数[i];
            }
            return cnts;
        }
        public double[] CalcCnts(double[] values, double[] entry_values, bool write = true)
        {
            for (int i = 0; i < (int)Type.护盾强效; i++)
            {
                if (i <= (int)Type.百分比防御力)
                {
                    if (values[i + (int)Type.基础生命值] == 0)
                    {
                        词条数[i] = 0;
                        if (write)
                        {
                            values[i + (int)Type.数值生命值] = entry_values[i + (int)Type.数值生命值] = values[i + (int)Type.总生命值] = entry_values[i + (int)Type.总生命值];
                        }
                    }
                    else if (单词条收益[i] == 0)
                    {
                        词条数[i] = 0;
                        if (write)
                            values[i] = entry_values[i] = (entry_values[i + (int)Type.总生命值] - values[i + (int)Type.数值生命值]) / values[i + (int)Type.基础生命值] - 1;
                    }
                    else
                    {
                        词条数[i] = (entry_values[i + (int)Type.总生命值] - values[i + (int)Type.总生命值]) / (单词条收益[i] * values[i + (int)Type.基础生命值]);
                    }
                }
                else
                {
                    if (单词条收益[i] == 0)
                    {
                        词条数[i] = 0;
                        if (write)
                            values[i] = entry_values[i];
                    }
                    else
                    {
                        词条数[i] = (entry_values[i] - values[i]) / 单词条收益[i];
                    }
                }
            }
            return 词条数;
        }
    }
    public struct CalcTrans
    {
        public int src, dest;
        public double value, start, end;
    }
    public struct CalcSkill
    {
        public string name;
        public struct Rate
        {
            public double[] values;
            public double[] trans_values;
            public List<CalcTrans> trans;
            public double times;
            public void ParseBuff(List<Buff> buffs)
            {
                foreach (var buff in buffs)
                {
                    if (buff.type == "生命值倍率%") { values[(int)RateType.生命值倍率] += buff.value * times / 100; }
                    else if (buff.type == "攻击力倍率%") { values[(int)RateType.攻击力倍率] += buff.value * times / 100; }
                    else if (buff.type == "防御力倍率%") { values[(int)RateType.防御力倍率] += buff.value * times / 100; }
                    else if (buff.type == "元素精通倍率%") { if (Options.mode == Options.原神) values[(int)RateType.元素精通倍率] += buff.value * times / 100; }
                    else if (buff.type == "速度倍率%") { if (Options.mode == Options.星穹铁道) values[(int)RateType.速度倍率] += buff.value * times / 100; }
                    else if (buff.type == "击破倍率%") { if (Options.mode == Options.星穹铁道) values[(int)RateType.击破倍率] += buff.value * times / 100 * 3767.8; }
                    else if (buff.type == "固定数值") { values[(int)RateType.固定数值] += buff.value * times; }
                }
            }
            public void CalcTrans(double[] base_status)
            {
                Array.Copy(values, trans_values, (int)RateType.len);
                foreach (var tr in trans)
                {
                    double src = base_status[tr.src] - tr.start;
                    if (src > 0)
                    {
                        if (src > tr.end && tr.end > tr.start)
                        {
                            src = tr.end;
                        }
                        trans_values[tr.dest] += src * tr.value;
                    }
                }
                if (Options.mode == Options.原神)
                {
                    trans_values[(int)RateType.速度倍率] = 0;
                    trans_values[(int)RateType.击破倍率] = 0;
                }
                else if (Options.mode == Options.星穹铁道)
                {
                    trans_values[(int)RateType.元素精通倍率] = 0;
                }
            }
            public double CalcDmg(double[] status)
            {
                //Debug.Log(String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", trans_values[0], trans_values[1], trans_values[2], trans_values[3], trans_values[4], status[(int)Type.总生命值], status[(int)Type.总攻击力], status[(int)Type.总防御力], status[(int)Type.元素精通]));
                return status[(int)Type.总生命值] * trans_values[(int)RateType.生命值倍率] +
                    status[(int)Type.总攻击力] * trans_values[(int)RateType.攻击力倍率] +
                    status[(int)Type.总防御力] * trans_values[(int)RateType.防御力倍率] +
                    status[(int)Type.元素精通] * trans_values[(int)RateType.元素精通倍率] +
                    status[(int)Type.速度] * trans_values[(int)RateType.速度倍率] +
                    trans_values[(int)RateType.固定数值];
            }
            public double Calc击破(double[] status)
            {
                return values[(int)RateType.击破倍率] * (1 + status[(int)Type.击破特攻]);
            }

            public Rate(List<Buff> buffs, double times)
            {
                values = new double[(int)RateType.len];
                trans_values = new double[(int)RateType.len];
                trans = default;
                this.times = times;
                ParseBuff(buffs);
            }
        }
        public struct React
        {
            public double 剧变基础伤害;
            public bool 激化, 剧变触发增幅;
            public double 增幅倍率, 增幅覆盖率;
            public React(SLStruct.React react)
            {
                if (Options.mode == Options.原神)
                {
                    var dict = ValueDict.react_types;
                    剧变基础伤害 = dict[react.trans.type] * react.trans.value;
                    激化 = react.trans.type.IndexOf("激化") > 0;
                    剧变触发增幅 = react.ta;
                    增幅倍率 = dict[react.amp.type];
                    增幅覆盖率 = react.amp.value / 100;
                }
                else
                {
                    剧变基础伤害 = 0;
                    激化 = false;
                    剧变触发增幅 = false;
                    增幅倍率 = 0;
                    增幅覆盖率 = 0;
                }
            }
            public double Calc激化(double[] status)
            {
                if (激化)
                {
                    return 剧变基础伤害 * (1 + 5 * status[(int)Type.元素精通] / (status[(int)Type.元素精通] + 1200) + status[(int)Type.剧变加成]);
                }
                else
                {
                    return 0;
                }
            }
            public double Calc增幅(double[] status)
            {
                if (增幅倍率 == 0)
                {
                    return 1;
                }
                return 增幅倍率 * (1 + 2.78 * status[(int)Type.元素精通] / (status[(int)Type.元素精通] + 1400) + status[(int)Type.增幅加成]) * 增幅覆盖率 + 1 - 增幅覆盖率;
            }
            public double Calc剧变(double[] status)
            {
                if (激化)
                {
                    return 0;
                }
                else
                {
                    return 剧变基础伤害 * (1 + 16 * status[(int)Type.元素精通] / (status[(int)Type.元素精通] + 2000) + status[(int)Type.剧变加成]);
                }
            }
        }

        public Rate rate;
        public React react;
        public Status status;
        public Other other;
        public double dmg;
        public CalcSkill(Skill skill, Status stat)
        {
            name = skill.name;
            rate = new Rate(skill.rate.normal, skill.rate.times);
            react = new React(skill.react);
            status = new Status(stat);
            status.ParseBuff(skill.buff);
            status.ParseTrans(skill.trans);
            other = skill.other;
            dmg = 0;
        }
        public double CalcDmg(double[] entry)
        {
            status.CalcEntry(entry);
            status.CalcTrans();
            rate.CalcTrans(status.trans_values);
            double[] values = status.trans_values;
            double 基础伤害 = rate.CalcDmg(values) + react.Calc激化(values);
            double 增幅倍率 = react.Calc增幅(values);
            double 剧变伤害 = react.Calc剧变(values);
            double 伤害加成 = other.inc ? (1 + values[(int)Type.伤害加成]) : 1;
            double 暴击期望 = other.crit ? (1 + Math.Min(Math.Max(values[(int)Type.暴击率], 0), 1) * values[(int)Type.暴击伤害]) : 1;
            double 防御乘区 = other.def ? Calc防御(values[(int)Type.无视防御]) : 1;
            double 抗性乘区 = other.def ? Calc抗性(Options.def_res - values[(int)Type.无视抗性]) : 1;

            double 治疗护盾 = (other.heal ? (1 + values[(int)Type.治疗加成]) : 1) * (other.shield ? (1 + values[(int)Type.护盾强效]) : 1);
            double 承伤倍率 = other.self ? (0.5 + values[(int)Type.总防御力] / Options.def_base) / Calc抗性(values[(int)Type.元素抗性]) : 1;
            //Debug.Log(string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", 基础伤害, 增幅倍率, 剧变伤害, 伤害加成, 暴击期望, 防御乘区, 抗性乘区, 治疗护盾, 承伤倍率));
            dmg = ((基础伤害 * 增幅倍率 * 伤害加成 * 暴击期望 * 治疗护盾 * 承伤倍率 + rate.Calc击破(values)) * 防御乘区 + 剧变伤害 * (react.剧变触发增幅 ? 增幅倍率 : 1)) * 抗性乘区 * (1 + values[(int)Type.敌方易伤]);
            return dmg;
        }
        public double Calc抗性(double 元素抗性) {
            if (元素抗性 < 0)
                return 1 - 元素抗性 / Options.res_div;//星铁负抗性收益不减半
            else if (元素抗性 > 0.75)
                return 1 / (4 * 元素抗性 + 1);
            else
                return 1 - 元素抗性;
        }
        public double Calc防御(double 无视防御)
        {
            //原神90级角色100级怪物，防御力基数950/1000
            //星铁80级角色95级怪物，防御力基数1000/1150
            return Options.mode == Options.原神 ? (19 / (39 - 20 * 无视防御)) : (20 / (43 - 23 * 无视防御));
        }
    }
    struct CalcData
    {
        public Status status;
        public Entry entry;
        public CalcSkill[] skill;
        public SLData backup;
        public CalcData(SLData data)
        {
            backup = data;
            status = new Status(data.character.baseHp + data.weapon.baseHp,
                data.character.baseAtk + data.weapon.baseAtk,
                data.character.baseDef + data.weapon.baseDef);
            status.ParseBuff(data.other);
            status.ParseBuff(data.main);

            entry = new Entry(data.sub);
            status.ParseEntry(entry);
            status.ParseTrans(data.trans);

            skill = new CalcSkill[data.skill.Count()];
            for (int i = 0; i < skill.Length; i++)
            {
                Skill s = data.skill[i];
                skill[i] = new CalcSkill(s, status);
                skill[i].rate.trans = skill[i].status.ParseTrans(s.rate.trans, null);
            }
        }
        public bool isEmpty()
        {
            return skill == null || skill.Length == 0;
        }
        public double CalcCur(double[] cur_values, bool isTrans = false)
        {
            if (isTrans)
            {
                for (int i = 0; i < cur_values.Length; i++)
                {
                    if (i <= (int)Type.百分比防御力)
                    {
                        status.entry_values[i + (int)Type.总生命值] += cur_values[i] - status.trans_values[i + (int)Type.总生命值];
                    }
                    else
                    {
                        status.entry_values[i] += cur_values[i] - status.trans_values[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < cur_values.Length; i++)
                {
                    if (i <= (int)Type.百分比防御力)
                    {
                        status.entry_values[i + (int)Type.总生命值] = cur_values[i];
                    }
                    else
                    {
                        status.entry_values[i] = cur_values[i];
                    }
                }
            }
            status.values[(int)Type.护盾强效] = status.entry_values[(int)Type.护盾强效];
            status.CalcTotal(status.values);
            entry.CalcCnts(status.values, status.entry_values);
            entry.CalcValues();
            return CalcCurEntry(null);
            //status.CalcEntry(entry.词条属性);
            //status.CalcTrans();

            //skill = new CalcSkill[backup.skill.Count()];
            //for (int i = 0; i < skill.Length; i++)
            //{
            //    Skill s = backup.skill[i];
            //    skill[i] = new CalcSkill(s, status);
            //    skill[i].rate.trans = skill[i].status.ParseTrans(s.rate.trans, null);
            //}
            //return CalcOnce();
        }
        public double CalcCurEntry(double[] cur_entrys)
        {
            if (cur_entrys != null)
            {
                Array.Copy(cur_entrys, entry.词条数, Math.Min(cur_entrys.Length, entry.词条数.Length));
                for (int i = 0; i < (int)Type.护盾强效; i++)
                {
                    entry.词条数[i] -= entry.手动词条数[i];
                }
            }
            entry.CalcValues();
            status.CalcEntry(entry.词条属性);
            status.CalcTrans();

            skill = new CalcSkill[backup.skill.Count()];
            for (int i = 0; i < skill.Length; i++)
            {
                Skill s = backup.skill[i];
                skill[i] = new CalcSkill(s, status);
                skill[i].rate.trans = skill[i].status.ParseTrans(s.rate.trans, null);
            }
            return CalcOnce();
        }
        public double CalcAuto(int threads = 1)
        {
            double step = 0.1, target_step = Options.calc_step / 2, origin = 0;
            if (!Options.calc_optimize && target_step >= 0.00005) step = target_step;
            entry.AverageCnts();
            double[] inc = new double[(int)Type.伤害加成], dec = new double[(int)Type.伤害加成];
            int[] inc_type = new int[(int)Type.伤害加成], dec_type = new int[(int)Type.伤害加成];
            int[] template = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int times = 0;
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            for (; ; )
            {
                origin = CalcOnce();
                CalcEntryDmg(inc, step, (int)Type.伤害加成);
                CalcEntryDmg(dec, -step, (int)Type.伤害加成);
                Array.Copy(template, inc_type, (int)Type.伤害加成);
                Array.Copy(template, dec_type, (int)Type.伤害加成);
                Array.Sort(inc, inc_type);
                Array.Sort(dec, dec_type);
                times++;
                for (int i = (int)Type.击破特攻; i >= 0; i--)
                {
                    int j = 0;
                    entry.词条数[inc_type[i]] += step;
                    for (; j < (int)Type.伤害加成; j++)
                    {
                        if (inc[i] - origin > origin - dec[i])
                        {
                            if (entry.词条数[dec_type[j]] < step)
                            {
                                continue;
                            }
                            entry.词条数[dec_type[j]] -= step;
                            if (CalcOnce((int)Type.伤害加成) > origin)
                            {
                                goto __continue;//保留本次分配的词条
                            }
                            entry.词条数[dec_type[j]] += step;
                        }
                        else
                        {
                            break;
                        }
                    }
                    entry.词条数[inc_type[i]] -= step;
                    if (j == 0)
                    {
                        continue;
                    }
                }
                //已达到对应精度，得出结果
                if (step == target_step)
                {
                    stopwatch.Stop();
                    CalcOnce();
                    status.CalcEntry(entry.词条属性);
                    status.CalcTrans();
                    if(Options.show_calc_box)
                        MessageBox.ShowBox_s("计算完成，共进行" + times + "次词条分配计算，耗时" + stopwatch.Elapsed.TotalMilliseconds + "毫秒");
                    return origin;
                }
                //提高精度再次计算
                step = Math.Max(step / 5, target_step);
            __continue:;
            }
        }

        public double CalcOnce(int end = (int)Type.护盾强效)
        {
            entry.CalcValues(end);
            double v = 0;
            for (int i = 0; i < skill.Length; i++)
            {
                v += skill[i].CalcDmg(entry.词条属性);
            }
            return v;
        }
        public double[] CalcRate(double step = 0.01, double[] array = null)
        {
            double origin = CalcOnce();
            double[] inc = array ?? new double[(int)Type.护盾强效];
            CalcEntryDmg(inc, step);
            for (int i = 0; i < (int)Type.护盾强效; i++)
            {
                inc[i] = inc[i] / origin - 1;
                if (double.IsNaN(inc[i])) inc[i] = 0;
            }
            return inc;
        }
        void CalcEntryDmg(double[] array, double step = 0.01, int end = (int)Type.护盾强效)
        {
            for (int i = 0; i < end; i++)
            {
                entry.词条数[i] += step;
                array[i] = CalcOnce();
                entry.词条数[i] -= step;
            }
        }
    }
    CalcData data = default;
    public void CalcAll()
    {
        SLData slData = CalcSL();
        CalcData calcData = new CalcData(slData);
        double dmg = calcData.CalcAuto();
        grad.SetDmg(calcData.skill, dmg, dmg / slData.len, 0);
        grad.SetValues(calcData.status.entry_values, calcData.status.trans_values, calcData.entry.TotalCnts(), calcData.CalcRate(1));
        if (!cur.isEmpty())
        {
            calcData.CalcCur(cur.GetValues());
        }
        cur.SetValues(calcData.status.entry_values, calcData.status.trans_values, calcData.entry.TotalCnts(), calcData.CalcRate(1));
        dmg = calcData.CalcOnce();
        cur.SetDmg(calcData.skill, dmg, dmg / slData.len, 0);
        data = calcData;
    }
    public void CalcCur()
    {
        if (!data.isEmpty() && !cur.isEmpty())
        {
            data.CalcCur(cur.GetValues(out var isTrans), isTrans);
            cur.SetValues(data.status.entry_values, data.status.trans_values, data.entry.TotalCnts(), data.CalcRate(1));
            double dmg = data.CalcOnce();
            cur.SetDmg(data.skill, dmg, dmg / data.backup.len, 0);
        }
    }
    public void CalcCurEntry()
    {
        if (!data.isEmpty() && !cur.isEmpty())
        {
            data.CalcCurEntry(cur.GetEntrys());
            cur.SetValues(data.status.entry_values, data.status.trans_values, data.entry.TotalCnts(), data.CalcRate(1));
            double dmg = data.CalcOnce();
            cur.SetDmg(data.skill, dmg, dmg / data.backup.len, 0);
        }
    }

    public SLData CalcSL()
    {
        SLData slData = new SLData();
        slData.character = (Base)character.GetValue();
        slData.weapon = (Base)weapon.GetValue();
        slData.other = (List<Buff>)other.GetValue<Buff>();
        slData.trans = (List<TransBuff>)trans.GetValue<TransBuff>();
        slData.main = (List<Buff>)main.GetValue<Buff>();
        slData.maindef = (List<Buff>)maindef.GetValue<Buff>();
        slData.sub = (List<Buff>)sub.GetValue<Buff>();
        slData.subdef = (List<Buff>)subdef.GetValue<Buff>();
        slData.skill = (List<Skill>)skill.GetValue<Skill>();
        double.TryParse(len.text.Length > 0 ? len.text : len.placeholder.GetComponent<TMP_Text>().text, out slData.len);
        slData.cur = cur.GetValuesList();

        json = JsonUtility.ToJson(slData);
        Debug.Log(json);
        return slData;
    }

    public void Load()
    {
        Load(json);
    }
    public void Load(string json)
    {
        SLData slData = JsonUtility.FromJson<SLData>(json);
        if (slData != null)
        {
            Combo.allow_show = false;
            character.SetValue(slData.character);
            weapon.SetValue(slData.weapon);
            other.SetValue(slData.other);
            trans.SetValue(slData.trans);
            main.SetValue(slData.main);
            maindef.SetValue(slData.maindef);
            sub.SetValue(slData.sub);
            subdef.SetValue(slData.subdef);
            skill.SetValue(slData.skill);
            len.text = slData.len.ToString();
            cur.isTrans = false;
            cur.SetValues(slData.cur);
            data = default;
            Combo.allow_show = true;
        }
    }
    public void ToggleMode()
    {
        grad.ToggleMode();
        cur.ToggleMode();
    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        Screen.fullScreen = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

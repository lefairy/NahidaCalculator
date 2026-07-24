using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public enum ValueType
{
    增益属性,
    主词条属性,
    副词条属性,
    可转化属性,
    倍率类型,
    增幅反应,
    剧变反应,
    面板属性,
    画质设置,
    计算模式
}

static public class ValueDict
{
    static public Dictionary<ValueType, string[]> buff_types = new Dictionary<ValueType, string[]> {
        { ValueType.增益属性, new string[] { "生命值", "生命值%", "攻击力", "攻击力%", "防御力", "防御力%", "元素精通", "速度", "暴击率%", "暴击伤害%", "充能效率%", "击破特攻%", "伤害加成%", "无视防御%", "无视抗性%", "敌方易伤%", "独立乘区%", "治疗加成%", "护盾强效%",  "元素抗性%", "伤害减免%", "剧变加成%", "增幅加成%", "伤害值提升", "反应伤害值提升", "反应基础伤害提升%", "反应层数", "笑点", "欢愉度%" } },
        { ValueType.主词条属性, new string[] { "生命值", "生命值%", "攻击力", "攻击力%", "防御力%", "元素精通", "速度", "暴击率%", "暴击伤害%", "充能效率%", "击破特攻%", "伤害加成%", "治疗加成%" } },
        { ValueType.副词条属性, new string[] { "生命值%", "攻击力%", "防御力%", "元素精通", "速度", "暴击率%", "暴击伤害%", "充能效率%", "击破特攻%" } },
        { ValueType.可转化属性, new string[] { "生命值", "攻击力", "防御力", "元素精通", "速度", "暴击率%", "暴击伤害%", "充能效率%", "击破特攻%", "伤害加成%", "敌方易伤%", "独立乘区%", "治疗加成%", "护盾强效%", "元素抗性%", "伤害减免%", "剧变加成%", "增幅加成%", "伤害值提升", "反应伤害值提升", "反应基础伤害提升%", "反应层数", "笑点", "欢愉度%" } },
        { ValueType.面板属性, new string[] { "生命值", "攻击力", "防御力", "元素精通", "速度", "暴击率%", "暴击伤害%", "充能效率%", "击破特攻%", "伤害加成%", "治疗加成%", "护盾强效%", "元素抗性%", "伤害减免%" } },
        { ValueType.倍率类型, new string[] { "生命值倍率%", "攻击力倍率%", "防御力倍率%", "元素精通倍率%", "速度倍率%", "击破倍率%", "欢愉倍率%", "固定数值" } },
        { ValueType.增幅反应, new string[] { "水蒸发", "火蒸发", "火融化", "冰融化", "月感电" , "月绽放", "月结晶", "星超导", "星扩散" } },
        { ValueType.剧变反应, new string[] { "扩散", "感电", "超导", "超载", "碎冰", "绽放", "超绽放", "烈绽放", "超激化", "蔓激化", "月感电", "月结晶", "星扩散(风)", "星扩散(冰)" } },
        { ValueType.画质设置, QualitySettings.names },
        { ValueType.计算模式, new string[] { "原神", "星穹铁道" } }
    };

    static public Dictionary<string, double> main_values = new Dictionary<string, double> {
        { "生命值", 4780 }, { "生命值%", 46.6 }, { "攻击力", 311 }, { "攻击力%", 46.6 }, { "防御力%", 58.3 }, { "元素精通", 186.5 }, { "速度", 25 }, { "暴击率%", 31.1 }, { "暴击伤害%", 62.2 }, { "充能效率%", 51.8 }, { "击破特攻%", 64.8 }, { "伤害加成%", 46.6 }, { "治疗加成%", 35.9 }
    };
    static public Dictionary<string, double> sub_values = new Dictionary<string, double> {
        { "生命值%", 4.955 }, { "攻击力%", 4.955 }, { "防御力%", 6.195 }, { "元素精通", 19.815 }, { "速度", 2.3 }, { "暴击率%", 3.305 }, { "暴击伤害%", 6.605 }, { "充能效率%", 5.505 }, { "击破特攻%", 5.83 }, { "伤害加成%", 4.955 }, { "治疗加成%", 3.815 }
    };
    static public List<SLStruct.Buff>[] def_main_values = new List<SLStruct.Buff>[2] {
        new List<SLStruct.Buff>() {
            new SLStruct.Buff { type = "生命值", value = 4780 },
            new SLStruct.Buff { type = "生命值%", value = 46.6 },
            new SLStruct.Buff { type = "攻击力", value = 311 },
            new SLStruct.Buff { type = "攻击力%", value = 46.6 },
            new SLStruct.Buff { type = "防御力%", value = 58.3 },
            new SLStruct.Buff { type = "元素精通", value = 186.5 },
            new SLStruct.Buff { type = "速度", value = 25 },
            new SLStruct.Buff { type = "暴击率%", value = 31.1 },
            new SLStruct.Buff { type = "暴击伤害%", value = 62.2 },
            new SLStruct.Buff { type = "充能效率%", value = 51.8 },
            new SLStruct.Buff { type = "击破特攻%", value = 64.8 },
            new SLStruct.Buff { type = "伤害加成%", value = 46.6 },
            new SLStruct.Buff { type = "治疗加成%", value = 35.9 }
        },
        new List<SLStruct.Buff>() {
            new SLStruct.Buff { type = "生命值", value = 705.6 },
            new SLStruct.Buff { type = "生命值%", value = 43.2 },
            new SLStruct.Buff { type = "攻击力", value = 352.8 },
            new SLStruct.Buff { type = "攻击力%", value = 43.2 },
            new SLStruct.Buff { type = "防御力%", value = 54 },
            new SLStruct.Buff { type = "元素精通", value = 186.5 },
            new SLStruct.Buff { type = "速度", value = 25 },
            new SLStruct.Buff { type = "暴击率%", value = 32.4 },
            new SLStruct.Buff { type = "暴击伤害%", value = 64.8 },
            new SLStruct.Buff { type = "充能效率%", value = 19.44 },
            new SLStruct.Buff { type = "击破特攻%", value = 64.8 },
            new SLStruct.Buff { type = "伤害加成%", value = 38.88 },
            new SLStruct.Buff { type = "治疗加成%", value = 34.56 }
        }
    };
    static public List<SLStruct.Buff>[] def_sub_values = new List<SLStruct.Buff>[2] {
        new List<SLStruct.Buff>() {
            new SLStruct.Buff { type = "生命值%", value = 4.955 },
            new SLStruct.Buff { type = "攻击力%", value = 4.955 },
            new SLStruct.Buff { type = "防御力%", value = 6.195 },
            new SLStruct.Buff { type = "元素精通", value = 19.815 },
            new SLStruct.Buff { type = "速度", value = 2.3 },
            new SLStruct.Buff { type = "暴击率%", value = 3.305 },
            new SLStruct.Buff { type = "暴击伤害%", value = 6.605 },
            new SLStruct.Buff { type = "充能效率%", value = 5.505 },
            new SLStruct.Buff { type = "击破特攻%", value = 5.83 },
            new SLStruct.Buff { type = "伤害加成%", value = 4.955 },
            new SLStruct.Buff { type = "治疗加成%", value = 3.815 }
        },
        new List<SLStruct.Buff>() {
            new SLStruct.Buff { type = "生命值%", value = 3.89 },
            new SLStruct.Buff { type = "攻击力%", value = 3.89 },
            new SLStruct.Buff { type = "防御力%", value = 4.86 },
            new SLStruct.Buff { type = "元素精通", value = 19.815 },
            new SLStruct.Buff { type = "速度", value = 2.3 },
            new SLStruct.Buff { type = "暴击率%", value = 2.917 },
            new SLStruct.Buff { type = "暴击伤害%", value = 5.83 },
            new SLStruct.Buff { type = "充能效率%", value = 1.75 },
            new SLStruct.Buff { type = "击破特攻%", value = 5.83 },
            new SLStruct.Buff { type = "伤害加成%", value = 3.5 },
            new SLStruct.Buff { type = "治疗加成%", value = 3.11 }
        }
    };

    static public Dictionary<string, int> status_types = new Dictionary<string, int> {
        { "生命值", (int)Calc.Type.数值生命值 }, { "生命值%", (int)Calc.Type.百分比生命值 }, { "攻击力", (int)Calc.Type.数值攻击力 },
        { "攻击力%", (int)Calc.Type.百分比攻击力 }, { "防御力", (int)Calc.Type.数值防御力 }, { "防御力%", (int)Calc.Type.百分比防御力 },
        { "元素精通", (int)Calc.Type.元素精通 }, { "暴击率%", (int)Calc.Type.暴击率 }, { "暴击伤害%", (int)Calc.Type.暴击伤害 },
        { "充能效率%", (int)Calc.Type.充能效率 }, { "伤害加成%", (int)Calc.Type.伤害加成 }, { "治疗加成%", (int)Calc.Type.治疗加成 },
        { "护盾强效%", (int)Calc.Type.护盾强效 }, { "元素抗性%", (int)Calc.Type.元素抗性 }, { "伤害减免%", (int)Calc.Type.伤害减免 },
        { "无视防御%", (int)Calc.Type.无视防御 }, { "无视抗性%", (int)Calc.Type.无视抗性 }, { "剧变加成%", (int)Calc.Type.剧变加成 },
        { "增幅加成%", (int)Calc.Type.增幅加成 }, { "速度", (int)Calc.Type.速度 }, { "击破特攻%", (int)Calc.Type.击破特攻 },
        { "敌方易伤%", (int)Calc.Type.敌方易伤 }, { "独立乘区%", (int)Calc.Type.独立乘区 }, { "伤害值提升", (int)Calc.Type.伤害值提升 },
        { "反应伤害值提升", (int)Calc.Type.反应伤害值提升 }, { "反应基础伤害提升%", (int)Calc.Type.反应基础伤害提升 }, { "反应层数", (int)Calc.Type.反应层数 },
        { "笑点", (int)Calc.Type.笑点 }, { "欢愉度%", (int)Calc.Type.欢愉度 },
        { "生命值倍率%", (int)Calc.RateType.生命值倍率 }, { "攻击力倍率%", (int)Calc.RateType.攻击力倍率 },
        { "防御力倍率%", (int)Calc.RateType.防御力倍率 }, { "元素精通倍率%", (int)Calc.RateType.元素精通倍率 },
        { "速度倍率%", (int)Calc.RateType.速度倍率 }, { "击破倍率%", (int)Calc.RateType.击破倍率 },
        { "欢愉倍率%", (int)Calc.RateType.欢愉倍率 }, { "固定数值", (int)Calc.RateType.固定数值 },
    };

    static public double[] break_base = { 0,
        54, 58, 62, 67.52638, 70.5094, 73.52282, 76.56605, 79.63846, 82.73947, 85.86844,
        91.49441, 97.06798, 102.5892, 108.0579, 113.4743, 118.8383, 124.1499, 129.4091, 134.6159, 139.7703,
        149.3323, 158.8011, 168.1768, 177.4594, 186.6489, 195.7452, 204.7484, 213.6585, 222.4754, 231.1992,
        246.4276, 261.181, 275.4733, 289.3179, 302.7275, 315.7144, 328.2905, 340.4671, 352.2554, 363.6658,
        408.124, 451.7883, 494.6798, 536.8188, 578.2249, 618.9172, 658.9138, 698.2326, 736.8905, 774.9041,
        871.06, 964.8706, 1056.421, 1145.791, 1233.058, 1318.297, 1401.575, 1482.961, 1562.518, 1640.307,
        1752.322, 1861.901, 1969.124, 2074.066, 2176.798, 2277.39, 2375.909, 2472.416, 2566.974, 2659.641,
        2780.304, 2898.602, 3014.603, 3128.373, 3239.976, 3349.473, 3456.924, 3562.384, 3665.91, 3767.553,
        3957.862, 4155.212, 4359.864, 4572.088, 4792.164, 5020.383, 5257.047, 5502.466, 5756.967, 6020.884,
        6294.565, 6578.373, 6872.682, 7177.881, 7494.371, 7822.572, 8162.917, 8515.854, 8881.849, 9261.387 
    };
    static public double[] react_base = { 0,
        17.165, 18.535, 19.904, 21.274, 22.645, 24.649, 26.640, 28.868, 31.367, 34.143,
        37.201, 40.659, 44.446, 48.563, 53.748, 59.081, 64.420, 69.724, 75.123, 80.584,
        86.112, 91.703, 97.244, 102.812, 108.409, 113.201, 118.102, 122.979, 129.727, 136.292,
        142.670, 149.029, 155.416, 161.825, 169.106, 176.518, 184.072, 191.709, 199.556, 207.382,
        215.398, 224.165, 233.502, 243.350, 256.063, 268.543, 281.526, 295.013, 309.067, 323.601,
        336.757, 350.530, 364.482, 378.619, 398.600, 416.398, 434.386, 452.951, 472.606, 492.884,
        513.568, 539.103, 565.510, 592.538, 624.443, 651.470, 679.496, 707.794, 736.671, 765.640,
        794.773, 824.677, 851.157, 877.742, 914.229, 946.746, 979.411, 1011.223, 1044.791, 1077.443,
        1109.997, 1142.976, 1176.369, 1210.184, 1253.835, 1288.952, 1325.484, 1363.456, 1405.097, 1446.853,
        1446.853, 1446.853, 1446.853, 1446.853, 1561.46, 1561.46, 1561.46, 1561.46, 1561.46, 1674.81
    };
    static public Dictionary<string, double> react_values = new Dictionary<string, double> {
        { "扩散", 0.6 }, { "感电", 2 }, { "超导", 1.5 }, { "超载", 2.75 }, { "碎冰", 3 }, { "绽放", 2 }, { "超绽放", 3 }, { "烈绽放", 3 },
        { "超激化", 1.15 }, { "蔓激化", 1.25 }, { "月感电", 3 }, { "月结晶", 1.6 }, { "星扩散(风)", 0.75 }, { "星扩散(冰)", 2 }, { "", 0 }
    };

    static public Dictionary<string, double> react_values_amp = new Dictionary<string, double> {
        { "月感电", 3}, { "月绽放", 1}, { "月结晶", 1.6}, { "星超导", 1 }, { "星扩散", 1 }, { "水蒸发", 2 }, { "火蒸发", 1.5 }, { "火融化", 2 }, { "冰融化", 1.5 }, { "", 0 }
    };

    static public Dictionary<string, Calc.ReactType> react_types = new Dictionary<string, Calc.ReactType> {
        { "扩散", Calc.ReactType.常规 }, { "感电", Calc.ReactType.常规 }, { "超导", Calc.ReactType.常规 },
        { "超载", Calc.ReactType.常规 }, { "碎冰", Calc.ReactType.常规 }, { "绽放", Calc.ReactType.常规 },
        { "超绽放", Calc.ReactType.常规 }, { "烈绽放", Calc.ReactType.常规 }, { "超激化", Calc.ReactType.激化 },
        { "蔓激化", Calc.ReactType.激化 }, { "月感电", Calc.ReactType.月反应 }, { "月绽放", Calc.ReactType.月反应 },
        { "月结晶", Calc.ReactType.月反应 }, { "星超导", Calc.ReactType.星超导 }, { "星扩散", Calc.ReactType.月反应 },
        { "星扩散(风)", Calc.ReactType.星扩散风 }, { "星扩散(冰)", Calc.ReactType.星扩散冰 },
        { "水蒸发", Calc.ReactType.常规 }, { "火蒸发", Calc.ReactType.常规 },
        { "火融化", Calc.ReactType.常规 }, { "冰融化", Calc.ReactType.常规 }, { "", Calc.ReactType.常规 }
    };

    static public string[] type_strings = { "生命值", "攻击力", "防御力", "元素精通", "速度", "暴击率", "暴击伤害", "充能效率", "击破特攻", "伤害加成", "治疗加成", "护盾强效" };

}

public class Options : MonoBehaviour
{
    [SerializeField] public Mod_InputField quality, fps, calcMode, step, save, path_len;
    [SerializeField] public Toggle entry_two, multi, calc_box, combo_keyboard, mode_box;
    SLValue maindef, subdef;

    public const int 原神 = (int)CalcMode.原神, 星穹铁道 = (int)CalcMode.星穹铁道;
    enum CalcMode { 原神, 星穹铁道 }
    static SLOption defOption = new SLOption()
    {
        quality = "最高",
        mode = "原神",
        fps = 120,
        path_len = 260,
        step = 1e-10,
        entry_two = false,
        multi = true,
        calc_box = true,
        combo_keyboard = false,
        mode_box = true,
    };
    static public SLOption slOption = defOption;
    public struct SLOption
    {
        public string quality, mode, save;
        public int fps, path_len;
        public double step;
        public bool entry_two, multi, calc_box, combo_keyboard, mode_box;
    }
    bool allow_set = true;

    static public bool force_entry_two = false, calc_multi = true, show_calc_box = true, allow_combo_keyboard = false, show_mode_box = true;
    static public int mode = 原神, save_path_len = 260, char_lv = 90, enemy_lv = 100;
    static public double def_res = 0.1, res_div = 2, calc_step = 1e-10;
    static public string save_path;

    public void SetQuality(TMP_Text input)
    {
        if (SetQuality(input.text, true)) slOption.quality = input.text;
    }
    public bool SetQuality(string quality, bool box = false)
    {
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            if (QualitySettings.names[i].Equals(quality))
            {
                QualitySettings.SetQualityLevel(i, true);
                if (box) MessageBox.ShowBox_s("画质级别已设置为: " + QualitySettings.names[i]);
                else this.quality.text = quality;
                return true;
            }
        }
        if (box) MessageBox.ShowBox_s("无效的画质名称: " + quality);
        return false;
    }

    public void SetFPS(Mod_InputField input)
    {
        if (allow_set)
        {
            allow_set = false;
            SLValue.TryParse(input, out int fps);
            if (fps <= 0) { fps = defOption.fps; input.text = ""; }
            if (SetFPS(fps, true)) slOption.fps = fps;
        }
    }
    public bool SetFPS(int fps, bool box = false)
    {
        if (fps > 0)
        {
            Application.targetFrameRate = fps;
            if (box) MessageBox.ShowBox_s("帧率已设置为: " + fps, delegate { allow_set = true; });
            else this.fps.text = fps.ToString();
            return true;
        }
        if (box) allow_set = true;
        return false;
    }

    public void SetPathLen(Mod_InputField input)
    {
        if (allow_set)
        {
            allow_set = false;
            SLValue.TryParse(input, out int path_len);
            if (path_len <= 0) { path_len = defOption.path_len; input.text = ""; }
            if (SetPathLen(path_len, true)) slOption.path_len = path_len;
        }
    }
    public bool SetPathLen(int path_len, bool box = false)
    {
        if (path_len > 0)
        {
            if (box) MessageBox.ShowBox_s("存档完整路径长度限制已设置为: " + path_len, delegate { allow_set = true; });
            else this.path_len.text = path_len.ToString();
            save_path_len = path_len;
            return true;
        }
        if (box) allow_set = true;
        return false;
    }

    public void SetStep(Mod_InputField input)
    {
        if (allow_set)
        {
            allow_set = false;
            SLValue.TryParse(input, out double step);
            if (step <= 0) { step = defOption.step; input.text = ""; }
            if (SetStep(step, true)) slOption.step = step;
        }
    }
    public bool SetStep(double step, bool box = false)
    {
        if (step > 0)
        {
            calc_step = step;
            if (box)
            {
                if (step > 0.1)
                    MessageBox.ShowBox_s("自动分配词条计算步长已设置为: " + step + "\r\n注意: 实际生效的步长最大为0.1", delegate { allow_set = true; });
                else
                    MessageBox.ShowBox_s("自动分配词条计算步长已设置为: " + step + "\r\n注意: 数值越小精度越高, 但计算耗时也越长", delegate { allow_set = true; });
            }
            else this.step.text = step.ToString();
            return true;
        }
        if (box) allow_set = true;
        return false;
    }


    public void SetEntryTwo(Toggle toggle)
    {
        if (SetEntryTwo(toggle.isOn, true)) slOption.entry_two = toggle.isOn;
    }
    public bool SetEntryTwo(bool on, bool box = false)
    {
        force_entry_two = on;
        if (!box)
            entry_two.isOn = on;
        return true;
    }
    public void SetMulti(Toggle toggle)
    {
        if (SetMulti(toggle.isOn, true)) slOption.multi = toggle.isOn;
    }
    public bool SetMulti(bool on, bool box = false)
    {
        calc_multi = on;
        if (!box)
            multi.isOn = on;
        else if (!on)
        {
            MessageBox.ShowBox_s("已关闭多起点词条分配\r\n注意: 极少数情况下词条分配可能陷入局部最优");
        }
        return true;
    }
    public void SetCalcBox(Toggle toggle)
    {
        if (SetCalcBox(toggle.isOn, true)) slOption.calc_box = toggle.isOn;
    }
    public bool SetCalcBox(bool on, bool box = false)
    {
        show_calc_box = on;
        if (!box)
            calc_box.isOn = on;
        return true;
    }
    public void SetComboKeyboard(Toggle toggle)
    {
        if (SetComboKeyboard(toggle.isOn, true)) slOption.combo_keyboard = toggle.isOn;
    }
    public bool SetComboKeyboard(bool on, bool box = false)
    {
        allow_combo_keyboard = on;
        if (!box)
            combo_keyboard.isOn = on;
        return true;
    }
    public void SetModeBox(Toggle toggle)
    {
        if (SetModeBox(toggle.isOn, true)) slOption.mode_box = toggle.isOn;
    }
    public bool SetModeBox(bool on, bool box = false)
    {
        show_mode_box = on;
        if (!box)
            mode_box.isOn = on;
        return true;
    }

    public void SetModeString(string mode)
    {
        if (SetMode(mode, false)) {
            slOption.mode = mode;
            if (show_mode_box)
                MessageBox.ShowBox_s("计算模式已设置为: " + mode);
        }
    }
    public void SetMode(TMP_Text input)
    {
        if (SetMode(input.text, true)) slOption.mode = input.text;
    }
    public bool SetMode(string mode, bool box = false)
    {
        bool ret = false;
        if (mode == "原神")
        {
            Options.mode = 原神;
            char_lv = 90;
            enemy_lv = 100;
            def_res = 0.1;
            res_div = 2;
            ret = true;
        }
        else if (mode == "星穹铁道")
        {
            Options.mode = 星穹铁道;
            char_lv = 80;
            enemy_lv = 95;
            def_res = 0;
            res_div = 1;
            ret = true;
        }
        if (ret)
        {
            GetComponent<Calc>().ToggleMode();
            if (box) MessageBox.ShowBox_s("计算模式已设置为: " + mode + "，已修改敌方默认抗性、负抗性收益、防御力基数和默认等级，在结果中" + (Options.mode == 原神 ? "显示元素精通，隐藏速度和击破特攻" : "显示速度和击破特攻，隐藏元素精通") + "（被隐藏的数值不会参与计算）");
            else calcMode.text = mode;
            return true;
        }
        if (box) MessageBox.ShowBox_s("无效的计算模式: " + mode);
        return false;
    }
    public void SetModeDef()
    {
        MessageBox.ShowBox_s("将重置主副词条默认值，注意这些值可能会被存档保存或覆盖", Do_SetModeDef, true);
    }
    void Do_SetModeDef()
    {
        if (!maindef)
        {
            Start();
            maindef.gameObject.GetComponent<AutoFillValue>().Start();
            subdef.gameObject.GetComponent<AutoFillValue>().Start();
        }
        maindef.SetValue(ValueDict.def_main_values[mode]);
        subdef.SetValue(ValueDict.def_sub_values[mode]);
    }

    public void SetSave(Mod_InputField input)
    {
        if (allow_set)
        {
            allow_set = false;
            SetSave(input.text, true);
        }
    }
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
    void Android_Permission()
    {
        Permission.RequestUserPermissions(new string[] { Permission.ExternalStorageRead, Permission.ExternalStorageWrite, "android.permission.MANAGE_EXTERNAL_STORAGE" });

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (currentActivity != null)
            {
                using (var intent = new AndroidJavaObject("android.content.Intent"))
                {
                    intent.Call<AndroidJavaObject>("setAction", "android.settings.MANAGE_ALL_FILES_ACCESS_PERMISSION");
                    currentActivity.Call("startActivity", intent);
                }
            }
        }
    }
#endif
    public bool SetSave(string path, bool box = false)
    {
        string f = "存档路径将更改为\r\n";
        if (path == null || path.Length == 0)
        {
            path = defOption.save;
            f = "存档路径将更改为默认路径\r\n";
        }
        try
        {
            path.Trim();
            path.TrimEnd(new char[] {'/', '\\'});
            if (!path.EndsWith("/Save") && !path.EndsWith("\\Save")) path += "/Save";
            path = Path.GetFullPath(path);
        }
        catch
        {
            MessageBox.ShowBox_s("存档路径无效", delegate { allow_set = true; });
            save.text = save_path;
            return false;
        }
        if (box)
        {
            MessageBox.ShowBox_s(f + path + "\r\n并迁移所有文件", delegate { Do_SetSavePath(path, true); allow_set = true; }, true, delegate { save.text = save_path; allow_set = true; });
        }
        else
            Do_SetSavePath(path);
        return true;
    }
    void Do_SetSavePath(string path, bool box = false)
    {
        try
        {
            SaveLoad sl = GetComponent<SaveLoad>();
            if (sl.CheckPath(path))
            {
                if (!Equals(Path.GetFullPath(save_path), path) && new DirectoryInfo(save_path).Exists)
                {
                    CopyDirectory(save_path, path);
                    Directory.Delete(save_path, true);
                }
                save_path = path;
                slOption.save = path;
                save.text = path;
                if (box)
                    MessageBox.ShowBox_s("修改存档路径成功，注意及时保存设置到本地文件");
            }
            else
            {
                save.text = save_path;
                if (box)
                {
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
                    MessageBox.ShowBox_s("修改存档路径失败，请检查是否已授权，点击确定前往授权", Android_Permission, true);
#else
                    MessageBox.ShowBox_s("修改存档路径失败，请检查路径是否合法或是否可读写");
#endif
                }
            }
        }
        catch { MessageBox.ShowBox_s("修改存档路径失败"); save.text = save_path; }
    }
    public static void CopyDirectory(string sourceDir, string targetDir)
    {
        DirectoryInfo diSource = new DirectoryInfo(sourceDir);
        DirectoryInfo diTarget = new DirectoryInfo(targetDir);

        if (!diTarget.Exists)
            diTarget.Create();

        foreach (FileInfo fi in diSource.GetFiles())
        {
            fi.CopyTo(Path.Combine(diTarget.FullName, fi.Name), true);
        }

        foreach (DirectoryInfo diChild in diSource.GetDirectories())
        {
            CopyDirectory(diChild.FullName, Path.Combine(diTarget.FullName, diChild.Name));
        }
    }
    public void SetSaveDef(string path)
    {
        save.text = defOption.save = slOption.save = save_path = path;
    }

    public void LoadOptions(SLOption slOption)
    {
        Combo.allow_show = false;
        SetQuality(slOption.quality);
        SetFPS(slOption.fps);
        SetEntryTwo(slOption.entry_two);
        SetMode(slOption.mode);
        SetStep(slOption.step);
        SetMulti(slOption.multi);
        SetCalcBox(slOption.calc_box);
        SetComboKeyboard(slOption.combo_keyboard);
        SetModeBox(slOption.mode_box);
        SetSave(slOption.save);
        SetPathLen(slOption.path_len);
        Combo.allow_show = true;
    }

    public void ResetOptions()
    {
        slOption = defOption;
        LoadOptions(slOption);
    }
    public void LoadOptions(string json, bool first = false)
    {
        slOption = JsonUtility.FromJson<SLOption>(json);
        LoadOptions(slOption);
        if (first) Do_SetModeDef();
    }
    public string SaveOptions()
    {
        return JsonUtility.ToJson(slOption);
    }

    public void Start()
    {
        maindef = GetComponent<Calc>().maindef;
        subdef = GetComponent<Calc>().subdef;
    }
    public void Update()
    {

    }
}

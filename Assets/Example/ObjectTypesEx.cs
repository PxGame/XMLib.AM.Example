/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/xiangmu110/XMLib/wiki
 * 创建时间: 2020/5/3 2:27:59
 */

using XMLib;

public class ConditionTypesAttribute : ObjectTypesAttribute
{
    public ConditionTypesAttribute() :
        base(typeof(ConditionConfig.GroundChecker)
            , typeof(ConditionConfig.AirChecker)
            , typeof(ConditionConfig.KeyCodeChecker)
            , typeof(ConditionConfig.VeclocityChecker))
    {
    }
}

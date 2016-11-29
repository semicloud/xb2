# xb2
cross fault data tool for national earthquake response support service of China
# 2016-11-11
今天就算是一个开始吧！

# 2016-11-13
1. 加入了消趋势（线性回归）算法，年周变算法；
2. 增加了DateValues类，这其实是一个工具类，可以灵活的生成DateValues；
3. 尝试调用了MathDotNet的Vector类，及其上面的Map方法可以进一步研究；

# 2016-11-20
1. 修正分幅图中的Area高度问题和Y轴坐标对齐的问题，分幅图中startY和span的单位现在都是px；

# 2016-11-28
1. 增加了分幅图可设置大小、正选、反选等功能；
2. 探寻了更改y轴刻度疏密程度的方法，设置YAxis的Intervals属性即可；目前是使用的`IntervalAutoMode.VariableCount;`来动态调整Y轴刻度疏密程度的；
3. 需要继续完成合并（2种）等操作；

# 2016-11-30
1. 实现了分幅图的编辑，标题、颜色、线的粗细等；听起来不难，但真是个麻烦事儿；
2. 标地震功能实现了一半，剩下的过几天再做吧，科研ING!
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

#2016-12-5
1. 集成了核心算法；
2. 消趋势那个集成的差不多了，现在就是在做软件和算法的图形接口；碎吧~

#2016-12-6
1. 消趋势算法集成完毕，目前已经可以实现输入参数-调用算法-出图展示的业务闭环（算法的图形接口已经做了框架了）；下一步就是精化这个闭环，做其他算法的时候能够快些；尽量满足用户的要求；
2. 目前把算法都加进去了；导出图形基本完成了（基本符合出版要求），支持Png和EMF两种格式的导出，其中EMF是矢量图形；做了一些重构工作，简化代码难度；

#2016-12-8
1. 初步集成了年周变算法，算法的集成先不弄了(甲方的接口可能不确定，我得先干能够确定的部分，省的白干)，先弄分幅图管理那边吧；

#2016-12-9
1. 分幅图管理中标地震功能基本完成，剩下的就是细节的调整；
2. 下一步的计划，继续完成分幅图管理的功能；与甲方商议算法接口，定死，不允许再变了；

#2017-1-3
1. 查看了合同，消趋势和年周变那边都需要多测项输入，做了多测项输入的一个demo，下周给他们演示；
2. 准备算法文档，对照做的demo，给甲方演示操作流程；

#2017-1-8
1. 进行了一些初步的重构操作，目前软件较复杂的地方在地震目录的查询和算法的封装接口上；
2. 做了一个简单的输入Demo，准备明日与甲方讨论；

#2017-1-10
1. 又进行了一些重构，基本实现了单测项算法的输入接口;
2. 多测项的接口正在实现中；

#2017-1-11
1. 基本完成了地震目录子库创建功能的重构工作，代码清爽了一些；

#2017-1-14
1. 基本完成了创建地震目录子库、删除地震目录子库的重构工作；
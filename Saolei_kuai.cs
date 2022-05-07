using System.Windows.Forms;

namespace GameBox
{
    class Fangkuai : Button
    {
        // 方便属性管理；
        // 即使不写新子类也可以完成，
        // 但是相应的需要为每一个属性生成和雷块相对应的值（一般写成 数组）；

        /// <summary>
        /// 是否为雷
        /// </summary>
        public bool Lei;

        /// <summary>
        /// 是否被标记
        /// </summary>
        public bool flag;

        /// <summary>
        /// 以 （int）0 为起始数横坐标
        /// </summary>
        public int X;

        /// <summary>
        /// 以 （int）0 为起始数纵坐标
        /// </summary>
        public int Y;

        /// <summary>
        /// 判定其是否被翻开
        /// </summary>
        public bool tfgetimage;
        ///该块是否检查（有几个雷）过，防止递归地发生；
        ///[问就是血的教训]（一定要，必须要考虑递归的可能性）
    }
}

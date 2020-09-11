// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System;

namespace Cure.WPF
{
    /// <summary>
    /// 一个支持统一和高斯分布的随机数生成器。
    /// </summary>
    internal class RandomEngine
    {
        private Random _random;
        private double? _anotherSample;

        public RandomEngine(long seed)
        {
            this.Initialize(seed);
        }

        private void Initialize(long seed)
            => this._random = new Random((int)seed);

        public double NextGaussian(double mean, double variance)
            => this.Gaussian() * variance + mean;

        public double NextUniform(double min, double max)
            => this.Uniform() * (max - min) + min;

        private double Uniform()
            => this._random.NextDouble();

        /// <summary>
        /// 使用 Box-Muller 转换的极坐标形式生成一对无关、标准、正态分布的随机数、零期望值和单位偏差。
        /// </summary>
        private double Gaussian()
        {
            if (this._anotherSample.HasValue)
            {
                double num = this._anotherSample.Value;
                this._anotherSample = new double?();
                return num;
            }
            double num1;
            double num2;
            double d;
            do
            {
                num1 = 2.0 * this.Uniform() - 1.0;
                num2 = 2.0 * this.Uniform() - 1.0;
                d = num1 * num1 + num2 * num2;
            }
            while (d >= 1.0);
            double num3 = Math.Sqrt(-2.0 * Math.Log(d) / d);
            this._anotherSample = new double?(num1 * num3);
            return num2 * num3;
        }
    }
}

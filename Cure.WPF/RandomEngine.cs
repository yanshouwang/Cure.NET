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
    class RandomEngine
    {
        Random _random;
        double? _anotherSample;

        public RandomEngine(long seed)
        {
            Initialize(seed);
        }

        void Initialize(long seed)
            => _random = new Random((int)seed);

        public double NextGaussian(double mean, double variance)
            => Gaussian() * variance + mean;

        public double NextUniform(double min, double max)
            => Uniform() * (max - min) + min;

        double Uniform()
            => _random.NextDouble();

        /// <summary>
        /// 使用 Box-Muller 转换的极坐标形式生成一对无关、标准、正态分布的随机数、零期望值和单位偏差。
        /// </summary>
        double Gaussian()
        {
            if (_anotherSample.HasValue)
            {
                var num = _anotherSample.Value;
                _anotherSample = new double?();
                return num;
            }
            double num1;
            double num2;
            double d;
            do
            {
                num1 = 2.0 * Uniform() - 1.0;
                num2 = 2.0 * Uniform() - 1.0;
                d = num1 * num1 + num2 * num2;
            }
            while (d >= 1.0);
            var num3 = Math.Sqrt(-2.0 * Math.Log(d) / d);
            _anotherSample = new double?(num1 * num3);
            return num2 * num3;
        }
    }
}

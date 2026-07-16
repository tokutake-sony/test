using PersonFlowSimulation;

// ============================================================
// 人流シミュレーション - 1000㎡のスペースで100人がランダムに移動
// Person Flow Simulation - 100 people moving randomly in a 1000㎡ space
// ============================================================

const int TotalSteps = 200;           // 総シミュレーションステップ数
const int PrintInterval = 20;         // ヒートマップ表示間隔

var simulation = new Simulation(
    spaceWidth: 31.62,                // √1000 ≈ 31.62 m
    spaceHeight: 31.62,
    personCount: 100,
    seed: 42
);

Console.WriteLine("=================================================");
Console.WriteLine("  人流シミュレーション (Person Flow Simulation)");
Console.WriteLine("  スペース: 1000㎡  人数: 100人  ステップ数: 200");
Console.WriteLine("=================================================");
Console.WriteLine();

for (int step = 0; step <= TotalSteps; step++)
{
    if (step % PrintInterval == 0)
    {
        Console.WriteLine($"--- ステップ {step,3} / {TotalSteps} ---");
        simulation.PrintHeatmap(gridCols: 32, gridRows: 16);
        simulation.PrintStats();
        Console.WriteLine();
    }

    if (step < TotalSteps)
        simulation.Step();
}

Console.WriteLine("=================================================");
Console.WriteLine("  累積人流ヒートマップ (Cumulative Flow Heatmap)");
Console.WriteLine("=================================================");
simulation.PrintCumulativeHeatmap(gridCols: 64, gridRows: 32);
Console.WriteLine();
Console.WriteLine("シミュレーション完了。");


using System;
using System.Linq;

var X = monteCarlo(.5f, 10_000);
var Y = poissonDist(5f);
Console.WriteLine(X2(Y, X));

float X2(float[] expected, float[] dist)
{
    float sum = 0;
    for (int i = 0; i < expected.Length; i++)
    {
        var diff = dist[i] - expected[i];
        sum += diff * diff / expected[i];
    }
    return sum;
}

float[] poissonDist(float lambda)
{
    int maxExpected = (int)(5 * lambda);
    var dist = new float[maxExpected];

    for (int k = 0; k < dist.Length; k++)
        dist[k] = poisson(lambda, k);
    
    return dist;
}

float poisson(float lambda, int k)
{
    float result = 1f;
    for (int i = 0; i < k; i++)
        result *= lambda / (i + 1);
    return result * MathF.Exp(-lambda);
}

float[] monteCarlo(float lambda, int k)
{
    const float time = 10f;
    int maxExpected = (int)(5 * time * lambda);
    var dist = new float[maxExpected];
    var process = new PoissonProcess(lambda);

    for (int i = 0; i < k; i++)
    {
        int matches = process.Next(time);
        if (matches >= maxExpected)
            continue;
        
        dist[matches]++;
    }

    for (int i = 0; i < dist.Length; i++)
        dist[i] /= k;

    return dist;
}

public class PoissonProcess(float lambda)
{
    const float dt = 1E-3f;
    public int Next()
    {
        int matches = 0;
        float expected = dt * lambda;
        
        while (Random.Shared.NextSingle() < expected)
            matches++;
        
        return matches;
    }

    public int Next(float time)
    {
        int count = 0;
        for (float t = 0; t < time; t += dt)
            count += Next();
        return count;
    }
}
using Unity.Burst;
using Random = Unity.Mathematics.Random;

namespace Unity.Audio.DSPGraphSamples
{
    [BurstCompile(CompileSynchronously = true)]
    public struct NoiseFilter : IAudioKernel<NoiseFilter.Parameters, NoiseFilter.Providers>
    {
        public enum Parameters
        {
            [ParameterDefault(0.0f)][ParameterRange(-1.0f, 1.0f)]
            Offset
        }

        public enum Providers
        {
        }

        Random m_Random;

        public void Initialize()
        {
        }

        public void Execute(ref ExecuteContext<Parameters, Providers> context)
        {
            if (context.Outputs.Count == 0)
                return;

            if (m_Random.state == 0)
                m_Random.InitState(2747636419u);

            var outputSampleBuffer = context.Outputs.GetSampleBuffer(0);
            var outputChannels = outputSampleBuffer.Channels;
            var parameters = context.Parameters;
            var inputCount = context.Inputs.Count;

            for (var channel = 0; channel < outputChannels; ++channel)
            {
                var outputBuffer = outputSampleBuffer.GetBuffer(channel);
                for (var i = 0; i < inputCount; i++)
                {
                    var inputBuff = context.Inputs.GetSampleBuffer(i).GetBuffer(channel);
                    for (var s = 0; s < outputBuffer.Length; s++)
                        outputBuffer[s] += inputBuff[s];
                }

                for (var s = 0; s < outputBuffer.Length; s++)
                    outputBuffer[s] += m_Random.NextFloat() * 2.0f - 1.0f + parameters.GetFloat(Parameters.Offset, s);
            }
        }

        public void Dispose()
        {
        }
    }
}

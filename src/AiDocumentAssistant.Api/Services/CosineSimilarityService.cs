namespace AiDocumentAssistant.Api.Services;

public class CosineSimilarityService
{
    public double Calculate(float[] first, float[] second)
    {
        if (first == null || second == null)
        {
            throw new ArgumentNullException(
                first == null ? nameof(first) : nameof(second));
        }

        if (first.Length == 0 || second.Length == 0)
        {
            throw new ArgumentException(
                "Embeddings cannot be empty.");
        }

        if (first.Length != second.Length)
        {
            throw new ArgumentException(
                "Embeddings must have the same number of dimensions.");
        }

        double dotProduct = 0;
        double firstMagnitude = 0;
        double secondMagnitude = 0;

        for (int i = 0; i < first.Length; i++)
        {
            dotProduct += first[i] * second[i];

            firstMagnitude += first[i] * first[i];

            secondMagnitude += second[i] * second[i];
        }

        if (firstMagnitude == 0 || secondMagnitude == 0)
        {
            throw new ArgumentException(
                "Embeddings cannot have zero magnitude.");
        }

        return dotProduct /
               (Math.Sqrt(firstMagnitude) *
                Math.Sqrt(secondMagnitude));
    }
}

/// <summary>
/// Modify the standard array distribution in order allow the elements indexation
/// with (0,0) index starting from the bottom left and (xMax, yMax) in the top right.
/// </summary>
public static class Array2DTools
{
	public static T[,] ToCartesianDistribution<T>(T[,] array)
	{
		T[,] cartesianArray = (T[,])array.Clone();
		
		for (int iRun = array.GetLength(0) - 1, i = 0; iRun >= 0; iRun--, i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				cartesianArray[j, i] = array[iRun, j];					
			}
		}
		return cartesianArray;
	}
}

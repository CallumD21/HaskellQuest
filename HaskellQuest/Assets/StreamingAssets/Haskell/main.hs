import Test.QuickCheck
import ConvertMessage
double x = 2*x
test :: Int -> Int
test x = 2*x
prop_test :: Int->Bool
prop_test x = double x == test x
main ::IO()
main = do
result <- quickCheckResult prop_test
putStrLn (convertMessage result)

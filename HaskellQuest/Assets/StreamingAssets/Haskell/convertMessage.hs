module ConvertMessage where

import Test.QuickCheck

convertMessage :: Result -> String
convertMessage Success{output=o, numTests =nt} = "+++ OK, passed " ++ show nt ++ " tests"
convertMessage Failure {reason="Falsifiable", failingTestCase=ftc} = "Failed with input: " ++ head ftc
convertMessage _ = "ERROR" 
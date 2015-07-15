# TagUnitTestFromTestlist
When migrating some old build definitions to a newer template we faced a scenario where we had test lists (*.vsmdi) and a bunch of test methods which weren't properly categorized. In our situation we wanted to run the CI-build with a filter saying "TestCategory=Unit test", but that meant to manually categorize hundreds of test methods..

#####Roslyn to the rescue
So in order to clean up, here's a small utility which reads a solution file, the vsmdi file and categorizes the tests accordingly. With no specified test list or category, it categorizes everything based on the vsmdi file. 

If you specify which test list in the vsmdi file you want to process, you can also specify a test category name. For instance one can tag all test methods in one list with [TestCategory("Unit test")], and another list with [TestCategory("Integration test")].

All test methods that are not in a test list will remain untouched.

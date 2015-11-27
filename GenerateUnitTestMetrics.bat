REM Create a 'GeneratedReports' folder if it does not exist
if not exist "GeneratedReports" mkdir "GeneratedReports"
 
REM Remove any previous test execution files to prevent issues overwriting
IF EXIST "GitStashTest.trx" del "GitStashTest.trx%"
 
REM Remove any previously created test output directories
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"
 
REM Run the tests against the targeted output
call :RunOpenCoverUnitTestMetrics
 
REM Generate the report output based on the test results
if %errorlevel% equ 0 (
 call :RunReportGeneratorOutput
)
 
REM Launch the report
if %errorlevel% equ 0 (
 call :RunLaunchReport
)
exit /b %errorlevel%
 
:RunOpenCoverUnitTestMetrics
"packages\OpenCover.4.6.210-rc\tools\OpenCover.Console.exe" ^
-register:user ^
-skipautoprops ^
-filter:"-[LibGit2Sharp]* -[GitWrapper]EnvDTE* -[GitStashTest]* +[GitWrapper]*  +[GitStash*]*" ^
-target:"%VS140COMNTOOLS%\..\IDE\mstest.exe" ^
-targetargs:"/testsettings:Coverage.runsettings /testcontainer:\"GitStashTest\bin\Debug\GitStashTest.dll\" /resultsfile:\"GitStashTest.trx\"" ^
-mergebyhash ^
-output:"GeneratedReports\GitStashTest.xml"
exit /b %errorlevel%
 
:RunReportGeneratorOutput
"packages\ReportGenerator.2.3.4.0\tools\ReportGenerator.exe" ^
-reports:"GeneratedReports\GitStashTest.xml" ^
-targetdir:"GeneratedReports\ReportGenerator Output"
exit /b %errorlevel%
 
:RunLaunchReport
start "report" "GeneratedReports\ReportGenerator Output\index.htm"
exit /b %errorlevel%
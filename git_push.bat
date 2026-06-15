@echo off
:: Change the window title
title Git Quick Push - RoboLight

echo ========================================
echo   Checking Git Status for RoboLight...
echo ========================================
git status
echo.

:: Stage all files
echo Staging all changes...
git add .
echo.

:: Prompt the user for a commit message
set /p commit_msg="Enter your commit message: "

:: Check if the message is empty, provide a default if it is
if "%commit_msg%"=="" set commit_msg=Quick updates

echo.
echo Committing changes...
git commit -m "%commit_msg%"
echo.

echo Pushing to GitHub...
git push origin main

echo.
echo ========================================
echo   Done! Your changes are live.
echo ========================================
pause
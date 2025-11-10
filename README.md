# PROG7312 — Part 1 Submission

## Student Details
- **Name:** Theolen Pillay  
- **Student Number:** ST10028920  

---

## Project Overview
This project is a municipal services reporting system built in **ASP.NET Core MVC**.  
It allows citizens to report issues, upload attachments, and receive feedback through a gamified user interface.  

---

## Features Implemented
1. **Custom Data Structure**  
   - Implemented `IssueStore` using a fixed-size array for storing issues.  
   - Provides `AddIssue()` and `GetAllIssues()` methods.  

2. **Issue Reporting Form**  
   - Location, Category, Description, and optional Attachment (image/PDF).  
   - Validation and error handling included.  

3. **Gamification Feedback**  
   - Progress bar with glow effect at 100%.  
   - Shake animation when toggling attachment.  
   - Motivational labels for user engagement.  

4. **Thank You Screen**  
   - Displays a unique reference code when a report is submitted.  

5. **User Interface**  
   - Modern Home page (hero, gradient background, cards).  
   - Navbar with logo and branding.  
   - Styled Privacy page with fade + rotating icon.  

---

## How to Run
1. Open the solution file: `MunicipalServicesMVC.sln`.  
2. Build the project in **Visual Studio 2022**.  
3. Run using **IIS Express** or **Kestrel**.  
4. Navigate to:  
   - `/Home/Index` → Home page.  
   - `/Issues/Create` → Report form.  
   - `/Issues/ThankYou/{code}` → Confirmation.  

---

## Demo Video
Unlisted YouTube Link: https://youtu.be/BVlH5M2Vcww?si=Ysre9YXwD_eMhvPA

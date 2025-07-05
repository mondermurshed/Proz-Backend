using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proz_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class StartPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    LastOnline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feedback_Types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    FeedbackType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback_Types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false),
                    sold_copies = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftInformationTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Shift_Starts = table.Column<TimeOnly>(type: "time", nullable: false),
                    Shift_Ends = table.Column<TimeOnly>(type: "time", nullable: false),
                    TotalHours = table.Column<int>(type: "int", nullable: true),
                    ShiftType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftInformationTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IdentityUsers_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeesTable_AspNetUsers_IdentityUsers_FK",
                        column: x => x.IdentityUsers_FK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginHistoryTable",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(39)", maxLength: 39, nullable: true),
                    ExtendedIdentityUsersDesktop_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistoryTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LoginHistoryTable_AspNetUsers_ExtendedIdentityUsersDesktop_FK",
                        column: x => x.ExtendedIdentityUsersDesktop_FK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalInformationTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LivingOnPrimaryPlace = table.Column<bool>(type: "bit", nullable: false),
                    IdentityUser_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalInformationTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalInformationTable_AspNetUsers_IdentityUser_FK",
                        column: x => x.IdentityUser_FK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokensTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokensTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokensTable_AspNetUsers_UserFK",
                        column: x => x.UserFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BreaksTimeTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreakStart = table.Column<TimeOnly>(type: "time", nullable: false),
                    BreakEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    BreakType = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Shift_FK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreaksTimeTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreaksTimeTable_ShiftInformationTable_Shift_FK",
                        column: x => x.Shift_FK,
                        principalTable: "ShiftInformationTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Manager_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentDepartment_FK = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentsTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentsTable_DepartmentsTable_ParentDepartment_FK",
                        column: x => x.ParentDepartment_FK,
                        principalTable: "DepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentsTable_EmployeesTable_Manager_FK",
                        column: x => x.Manager_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeedbacksTable",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    FeedbackTitle = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FeedbackDescription = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEditAble = table.Column<bool>(type: "bit", nullable: false),
                    CanBeSeen = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    FeedbackType_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Employee_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbacksTable", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeedbacksTable_EmployeesTable_Employee_FK",
                        column: x => x.Employee_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedbacksTable_Feedback_Types_FeedbackType_FK",
                        column: x => x.FeedbackType_FK,
                        principalTable: "Feedback_Types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequestsTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    StartDate = table.Column<DateOnly>(type: "date", maxLength: 35, nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasSanctions = table.Column<bool>(type: "bit", nullable: true),
                    AgreedOn = table.Column<bool>(type: "bit", nullable: true),
                    DMStatus = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    FinalStatus = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentManagerComment = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FinalStatus_Comment = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Decision_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Requester_Employee_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentManager_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HandlerEmployee_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequestsTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequestsTable_EmployeesTable_DepartmentManager_FK",
                        column: x => x.DepartmentManager_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveRequestsTable_EmployeesTable_HandlerEmployee_FK",
                        column: x => x.HandlerEmployee_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveRequestsTable_EmployeesTable_Requester_Employee_FK",
                        column: x => x.Requester_Employee_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Seen_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    Target_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationsTable_EmployeesTable_Target_FK",
                        column: x => x.Target_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrentAddressTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Latitude_Coordinate = table.Column<double>(type: "float", nullable: true),
                    Longitude_Coordinate = table.Column<double>(type: "float", nullable: true),
                    Describe_The_Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PersonalInformation_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentAddressTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentAddressTable_PersonalInformationTable_PersonalInformation_FK",
                        column: x => x.PersonalInformation_FK,
                        principalTable: "PersonalInformationTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthInformationTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalConditions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CountryCodeOfThePhone = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    PersonalInformation_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthInformationTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthInformationTable_PersonalInformationTable_PersonalInformation_FK",
                        column: x => x.PersonalInformation_FK,
                        principalTable: "PersonalInformationTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalPhoneNumbersTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryNumber = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NumberType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PersonalInformation_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalPhoneNumbersTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalPhoneNumbersTable_PersonalInformationTable_PersonalInformation_FK",
                        column: x => x.PersonalInformation_FK,
                        principalTable: "PersonalInformationTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentContactMethodsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContactDetail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Department_FK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentContactMethodsTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentContactMethodsTable_DepartmentsTable_Department_FK",
                        column: x => x.Department_FK,
                        principalTable: "DepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDepartmentsTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Salary = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    Salary_Currency_Type = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Company_Bonus = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: true),
                    Company_Bonus_Currency_Type = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Payment_Frequency = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Employee_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Department_FK = table.Column<int>(type: "int", nullable: false),
                    Shift_FK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDepartmentsTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDepartmentsTable_DepartmentsTable_Department_FK",
                        column: x => x.Department_FK,
                        principalTable: "DepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDepartmentsTable_EmployeesTable_Employee_FK",
                        column: x => x.Employee_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDepartmentsTable_ShiftInformationTable_Shift_FK",
                        column: x => x.Shift_FK,
                        principalTable: "ShiftInformationTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbacksAnswersTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Answer = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    ResponseDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEditAble = table.Column<bool>(type: "bit", nullable: false),
                    Feedback_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RespondentAccount_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbacksAnswersTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbacksAnswersTable_EmployeesTable_RespondentAccount_FK",
                        column: x => x.RespondentAccount_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedbacksAnswersTable_FeedbacksTable_Feedback_FK",
                        column: x => x.Feedback_FK,
                        principalTable: "FeedbacksTable",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecorder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckInTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    CheckOutTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CheckInStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CheckOutStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CheckInComment = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    CheckOutComment = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    EmployeeDepartment_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecorder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecorder_EmployeeDepartmentsTable_EmployeeDepartment_FK",
                        column: x => x.EmployeeDepartment_FK,
                        principalTable: "EmployeeDepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Performed_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    PerformerAccount_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetEntity_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogsTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogsTable_EmployeeDepartmentsTable_TargetEntity_FK",
                        column: x => x.TargetEntity_FK,
                        principalTable: "EmployeeDepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogsTable_EmployeesTable_PerformerAccount_FK",
                        column: x => x.PerformerAccount_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryHistoryTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Salary = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    CurrencyType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PaymentFrequency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    EmployeeDepartments_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaryHistoryTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryHistoryTable_EmployeeDepartmentsTable_EmployeeDepartments_FK",
                        column: x => x.EmployeeDepartments_FK,
                        principalTable: "EmployeeDepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRecordsTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    PaymentDateCreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentPeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentPeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentCounter = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    SalaryCurrencyType = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FixedBonus = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    FixedBonusCurrencyType = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FixedBonusNote = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PerformanceBonus = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    PerformanceBonusCurrencyType = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    PerformanceBonusNote = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Deduction = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    DeductionCurrencyType = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DeductionNote = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeDepartments_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRecordsTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRecordsTable_EmployeeDepartmentsTable_EmployeeDepartments_FK",
                        column: x => x.EmployeeDepartments_FK,
                        principalTable: "EmployeeDepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceRecorderTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerformanceRating = table.Column<int>(type: "int", nullable: false),
                    ReviewerComment = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeDepartment_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reviewer_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceRecorderTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceRecorderTable_EmployeeDepartmentsTable_EmployeeDepartment_FK",
                        column: x => x.EmployeeDepartment_FK,
                        principalTable: "EmployeeDepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceRecorderTable_EmployeesTable_Reviewer_FK",
                        column: x => x.Reviewer_FK,
                        principalTable: "EmployeesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalaryScheduleTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CurrentPeriodStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrentPeriodEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentCounter = table.Column<int>(type: "int", nullable: false),
                    EmployeeBonus_Amount = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: true),
                    EmployeeBonus_Currency = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Employee_Deduction_Amount = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: true),
                    Employee_Deduction_Currency = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    EmployeeDepartment_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryScheduleTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryScheduleTable_EmployeeDepartmentsTable_EmployeeDepartment_FK",
                        column: x => x.EmployeeDepartment_FK,
                        principalTable: "EmployeeDepartmentsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecorder_EmployeeDepartment_FK",
                table: "AttendanceRecorder",
                column: "EmployeeDepartment_FK");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsTable_PerformerAccount_FK",
                table: "AuditLogsTable",
                column: "PerformerAccount_FK");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsTable_TargetEntity_FK",
                table: "AuditLogsTable",
                column: "TargetEntity_FK");

            migrationBuilder.CreateIndex(
                name: "IX_BreaksTimeTable_Shift_FK",
                table: "BreaksTimeTable",
                column: "Shift_FK");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentAddressTable_PersonalInformation_FK",
                table: "CurrentAddressTable",
                column: "PersonalInformation_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentContactMethodsTable_Department_FK",
                table: "DepartmentContactMethodsTable",
                column: "Department_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentsTable_Manager_FK",
                table: "DepartmentsTable",
                column: "Manager_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentsTable_ParentDepartment_FK",
                table: "DepartmentsTable",
                column: "ParentDepartment_FK");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDepartmentsTable_Department_FK",
                table: "EmployeeDepartmentsTable",
                column: "Department_FK");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDepartmentsTable_Employee_FK",
                table: "EmployeeDepartmentsTable",
                column: "Employee_FK");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDepartmentsTable_Shift_FK",
                table: "EmployeeDepartmentsTable",
                column: "Shift_FK");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryHistoryTable_EmployeeDepartments_FK",
                table: "EmployeeSalaryHistoryTable",
                column: "EmployeeDepartments_FK");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesTable_IdentityUsers_FK",
                table: "EmployeesTable",
                column: "IdentityUsers_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbacksAnswersTable_Feedback_FK",
                table: "FeedbacksAnswersTable",
                column: "Feedback_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbacksAnswersTable_RespondentAccount_FK",
                table: "FeedbacksAnswersTable",
                column: "RespondentAccount_FK");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbacksTable_Employee_FK",
                table: "FeedbacksTable",
                column: "Employee_FK");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbacksTable_FeedbackType_FK",
                table: "FeedbacksTable",
                column: "FeedbackType_FK");

            migrationBuilder.CreateIndex(
                name: "IX_HealthInformationTable_PersonalInformation_FK",
                table: "HealthInformationTable",
                column: "PersonalInformation_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestsTable_DepartmentManager_FK",
                table: "LeaveRequestsTable",
                column: "DepartmentManager_FK");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestsTable_HandlerEmployee_FK",
                table: "LeaveRequestsTable",
                column: "HandlerEmployee_FK");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestsTable_Requester_Employee_FK",
                table: "LeaveRequestsTable",
                column: "Requester_Employee_FK");

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistoryTable_ExtendedIdentityUsersDesktop_FK",
                table: "LoginHistoryTable",
                column: "ExtendedIdentityUsersDesktop_FK");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsTable_Target_FK",
                table: "NotificationsTable",
                column: "Target_FK");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecordsTable_EmployeeDepartments_FK",
                table: "PaymentRecordsTable",
                column: "EmployeeDepartments_FK");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRecorderTable_EmployeeDepartment_FK",
                table: "PerformanceRecorderTable",
                column: "EmployeeDepartment_FK");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRecorderTable_Reviewer_FK",
                table: "PerformanceRecorderTable",
                column: "Reviewer_FK");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInformationTable_IdentityUser_FK",
                table: "PersonalInformationTable",
                column: "IdentityUser_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalPhoneNumbersTable_PersonalInformation_FK",
                table: "PersonalPhoneNumbersTable",
                column: "PersonalInformation_FK");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokensTable_UserFK",
                table: "RefreshTokensTable",
                column: "UserFK");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryScheduleTable_EmployeeDepartment_FK",
                table: "SalaryScheduleTable",
                column: "EmployeeDepartment_FK",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AttendanceRecorder");

            migrationBuilder.DropTable(
                name: "AuditLogsTable");

            migrationBuilder.DropTable(
                name: "BreaksTimeTable");

            migrationBuilder.DropTable(
                name: "CurrentAddressTable");

            migrationBuilder.DropTable(
                name: "DepartmentContactMethodsTable");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryHistoryTable");

            migrationBuilder.DropTable(
                name: "FeedbacksAnswersTable");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "HealthInformationTable");

            migrationBuilder.DropTable(
                name: "LeaveRequestsTable");

            migrationBuilder.DropTable(
                name: "LoginHistoryTable");

            migrationBuilder.DropTable(
                name: "NotificationsTable");

            migrationBuilder.DropTable(
                name: "PaymentRecordsTable");

            migrationBuilder.DropTable(
                name: "PerformanceRecorderTable");

            migrationBuilder.DropTable(
                name: "PersonalPhoneNumbersTable");

            migrationBuilder.DropTable(
                name: "RefreshTokensTable");

            migrationBuilder.DropTable(
                name: "SalaryScheduleTable");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FeedbacksTable");

            migrationBuilder.DropTable(
                name: "PersonalInformationTable");

            migrationBuilder.DropTable(
                name: "EmployeeDepartmentsTable");

            migrationBuilder.DropTable(
                name: "Feedback_Types");

            migrationBuilder.DropTable(
                name: "DepartmentsTable");

            migrationBuilder.DropTable(
                name: "ShiftInformationTable");

            migrationBuilder.DropTable(
                name: "EmployeesTable");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}

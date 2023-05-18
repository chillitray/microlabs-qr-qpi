using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OneMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    emp_id = table.Column<string>(type: "longtext", nullable: false),
                    joined_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    full_name = table.Column<string>(type: "longtext", nullable: true),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: true),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false),
                    verification_status = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CounterfietManagement",
                columns: table => new
                {
                    counterfeit_mgmt_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    counterfeit_type = table.Column<int>(type: "int", nullable: false),
                    low_risk_threshold = table.Column<int>(type: "int", nullable: false),
                    moderate_threshold = table.Column<int>(type: "int", nullable: false),
                    high_risk_threshold = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterfietManagement", x => x.counterfeit_mgmt_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    notification_content = table.Column<string>(type: "longtext", nullable: false),
                    notification_type = table.Column<Guid>(type: "char(36)", nullable: false),
                    redirect_url = table.Column<string>(type: "longtext", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.notification_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationActivity",
                columns: table => new
                {
                    notification_activity_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    notification_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    first_read_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    read_count = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationActivity", x => x.notification_activity_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationTypeManagement",
                columns: table => new
                {
                    notification_type_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    notifiication_type = table.Column<string>(type: "longtext", nullable: false),
                    priority = table.Column<int>(type: "int", nullable: false),
                    notification_for = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypeManagement", x => x.notification_type_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Plant",
                columns: table => new
                {
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    plant_name = table.Column<string>(type: "longtext", nullable: false),
                    plant_code = table.Column<string>(type: "longtext", nullable: false),
                    plant_description = table.Column<string>(type: "longtext", nullable: false),
                    plant_location_address = table.Column<string>(type: "longtext", nullable: false),
                    plant_location_city = table.Column<string>(type: "longtext", nullable: false),
                    plant_location_state = table.Column<string>(type: "longtext", nullable: false),
                    plant_location_country = table.Column<string>(type: "longtext", nullable: false),
                    plant_location_pincode = table.Column<string>(type: "longtext", nullable: false),
                    plant_location_geo = table.Column<string>(type: "longtext", nullable: false),
                    plant_qr_limit = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    operated_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    founded_on = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant", x => x.plant_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlantKeyManagement",
                columns: table => new
                {
                    plant_key_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    plant_key = table.Column<string>(type: "longtext", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantKeyManagement", x => x.plant_key_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlantSessionManagement",
                columns: table => new
                {
                    plant_session_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    plant_key = table.Column<string>(type: "longtext", nullable: false),
                    plant_access_token = table.Column<string>(type: "longtext", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expired_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_access = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_access_ip = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantSessionManagement", x => x.plant_session_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductManagement",
                columns: table => new
                {
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_name = table.Column<string>(type: "longtext", nullable: false),
                    product_description = table.Column<string>(type: "longtext", nullable: false),
                    product_logo = table.Column<string>(type: "longtext", nullable: true),
                    product_writeup = table.Column<string>(type: "longtext", nullable: true),
                    product_expiry_days = table.Column<int>(type: "int", nullable: false),
                    product_mrp = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductManagement", x => x.product_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductMedia",
                columns: table => new
                {
                    product_media_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    url = table.Column<string>(type: "longtext", nullable: false),
                    media_type = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMedia", x => x.product_media_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductPlantMapping",
                columns: table => new
                {
                    product_plant_mapping_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPlantMapping", x => x.product_plant_mapping_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QrManagement",
                columns: table => new
                {
                    qr_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    public_id = table.Column<string>(type: "longtext", nullable: false),
                    manufactured_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    product_mrp_copy = table.Column<int>(type: "int", nullable: true),
                    pack_id = table.Column<string>(type: "longtext", nullable: true),
                    serial_number = table.Column<string>(type: "longtext", nullable: true),
                    batch_no = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at_ip = table.Column<string>(type: "longtext", nullable: false),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrManagement", x => x.qr_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QrReadActivity",
                columns: table => new
                {
                    qr_read_activity_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    qr_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ip_address = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrReadActivity", x => x.qr_read_activity_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RangeTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    last_used_value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangeTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RateLimits",
                columns: table => new
                {
                    rate_limit_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    rate_type = table.Column<int>(type: "int", nullable: false),
                    max_allowed_per_day = table.Column<int>(type: "int", nullable: false),
                    max_allowed_overall = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateLimits", x => x.rate_limit_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RequestProductAccess",
                columns: table => new
                {
                    request_product_access_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: true),
                    requested_qr_limit = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    requested_by_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    responded_by_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestProductAccess", x => x.request_product_access_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    role = table.Column<string>(type: "longtext", nullable: false),
                    access_level = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: true),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.role_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SessionActivity",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_access_token = table.Column<string>(type: "longtext", nullable: false),
                    last_login = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_login_ip = table.Column<string>(type: "longtext", nullable: false),
                    last_access = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_access_ip = table.Column<string>(type: "longtext", nullable: false),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true),
                    expired_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionActivity", x => x.session_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SmtpConfig",
                columns: table => new
                {
                    smtp_config_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    max_emails_per_day = table.Column<int>(type: "int", nullable: false),
                    email_id = table.Column<string>(type: "longtext", nullable: false),
                    password = table.Column<string>(type: "longtext", nullable: false),
                    email_type = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmtpConfig", x => x.smtp_config_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackerEmail",
                columns: table => new
                {
                    tracker_email_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: false),
                    email_subject = table.Column<string>(type: "longtext", nullable: false),
                    email_body = table.Column<string>(type: "longtext", nullable: false),
                    reason = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackerEmail", x => x.tracker_email_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackerOtp",
                columns: table => new
                {
                    tracker_otp_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: true),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    otp = table.Column<string>(type: "longtext", nullable: false),
                    reason = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    failed_attempts = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_attempted_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackerOtp", x => x.tracker_otp_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingActivity",
                columns: table => new
                {
                    tracking_activity_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    custom_obj = table.Column<string>(type: "longtext", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: false),
                    severity_type = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingActivity", x => x.tracking_activity_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingApi",
                columns: table => new
                {
                    tracking_api_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    req_method = table.Column<int>(type: "int", nullable: false),
                    req_endpoint = table.Column<string>(type: "longtext", nullable: false),
                    req_headers = table.Column<string>(type: "longtext", nullable: false),
                    request_obj = table.Column<string>(type: "longtext", nullable: false),
                    response_obj = table.Column<string>(type: "longtext", nullable: true),
                    request_type = table.Column<int>(type: "int", nullable: false),
                    ip_address = table.Column<string>(type: "longtext", nullable: true),
                    unique_id = table.Column<string>(type: "longtext", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingApi", x => x.tracking_api_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingCounterfeitManagement",
                columns: table => new
                {
                    tracking_counterfeit_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    counterfeit_mgmt_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingCounterfeitManagement", x => x.tracking_counterfeit_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingEditSmtpConfig",
                columns: table => new
                {
                    tracking_edit_smtp_config_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    smtp_config_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingEditSmtpConfig", x => x.tracking_edit_smtp_config_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingNotification",
                columns: table => new
                {
                    tracking_notification_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    notification_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingNotification", x => x.tracking_notification_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingNotificationManagement",
                columns: table => new
                {
                    tracking_notification_mgmt_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    notification_type_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingNotificationManagement", x => x.tracking_notification_mgmt_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingPlantActivity",
                columns: table => new
                {
                    tracking_plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    plant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingPlantActivity", x => x.tracking_plant_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingPlantKeyManagement",
                columns: table => new
                {
                    tracking_plant_key_mgmt_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    plant_key_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingPlantKeyManagement", x => x.tracking_plant_key_mgmt_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingProductActivity",
                columns: table => new
                {
                    tracking_product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingProductActivity", x => x.tracking_product_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingProductMediaActivity",
                columns: table => new
                {
                    tracking_product_media_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    product_media_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingProductMediaActivity", x => x.tracking_product_media_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingProductPlantMapActivity",
                columns: table => new
                {
                    tracking_product_plant_map_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    product_plant_mapping_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingProductPlantMapActivity", x => x.tracking_product_plant_map_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingQrManagement",
                columns: table => new
                {
                    tracking_qr_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    qr_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingQrManagement", x => x.tracking_qr_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingRateLimit",
                columns: table => new
                {
                    tracking_rate_limit_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    request_obj = table.Column<string>(type: "longtext", nullable: false),
                    response_obj = table.Column<string>(type: "longtext", nullable: true),
                    current_count = table.Column<int>(type: "int", nullable: false),
                    request_type = table.Column<int>(type: "int", nullable: false),
                    unique_id = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingRateLimit", x => x.tracking_rate_limit_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrackingUserEditActivity",
                columns: table => new
                {
                    tracking_user_edit_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    old_obj = table.Column<string>(type: "longtext", nullable: false),
                    new_obj = table.Column<string>(type: "longtext", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingUserEditActivity", x => x.tracking_user_edit_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserInformation",
                columns: table => new
                {
                    user_information_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    full_name = table.Column<string>(type: "longtext", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: false),
                    phone_no = table.Column<string>(type: "longtext", nullable: true),
                    country_code = table.Column<string>(type: "longtext", nullable: false),
                    location_address = table.Column<string>(type: "longtext", nullable: false),
                    location_city = table.Column<string>(type: "longtext", nullable: false),
                    location_state = table.Column<string>(type: "longtext", nullable: false),
                    location_country = table.Column<string>(type: "longtext", nullable: false),
                    location_pincode = table.Column<string>(type: "longtext", nullable: true),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    qr_read_activity_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    qr_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ip_address = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInformation", x => x.user_information_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

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
                name: "CounterfietManagement");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "NotificationActivity");

            migrationBuilder.DropTable(
                name: "NotificationTypeManagement");

            migrationBuilder.DropTable(
                name: "Plant");

            migrationBuilder.DropTable(
                name: "PlantKeyManagement");

            migrationBuilder.DropTable(
                name: "PlantSessionManagement");

            migrationBuilder.DropTable(
                name: "ProductManagement");

            migrationBuilder.DropTable(
                name: "ProductMedia");

            migrationBuilder.DropTable(
                name: "ProductPlantMapping");

            migrationBuilder.DropTable(
                name: "QrManagement");

            migrationBuilder.DropTable(
                name: "QrReadActivity");

            migrationBuilder.DropTable(
                name: "RangeTable");

            migrationBuilder.DropTable(
                name: "RateLimits");

            migrationBuilder.DropTable(
                name: "RequestProductAccess");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "SessionActivity");

            migrationBuilder.DropTable(
                name: "SmtpConfig");

            migrationBuilder.DropTable(
                name: "TrackerEmail");

            migrationBuilder.DropTable(
                name: "TrackerOtp");

            migrationBuilder.DropTable(
                name: "TrackingActivity");

            migrationBuilder.DropTable(
                name: "TrackingApi");

            migrationBuilder.DropTable(
                name: "TrackingCounterfeitManagement");

            migrationBuilder.DropTable(
                name: "TrackingEditSmtpConfig");

            migrationBuilder.DropTable(
                name: "TrackingNotification");

            migrationBuilder.DropTable(
                name: "TrackingNotificationManagement");

            migrationBuilder.DropTable(
                name: "TrackingPlantActivity");

            migrationBuilder.DropTable(
                name: "TrackingPlantKeyManagement");

            migrationBuilder.DropTable(
                name: "TrackingProductActivity");

            migrationBuilder.DropTable(
                name: "TrackingProductMediaActivity");

            migrationBuilder.DropTable(
                name: "TrackingProductPlantMapActivity");

            migrationBuilder.DropTable(
                name: "TrackingQrManagement");

            migrationBuilder.DropTable(
                name: "TrackingRateLimit");

            migrationBuilder.DropTable(
                name: "TrackingUserEditActivity");

            migrationBuilder.DropTable(
                name: "UserInformation");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}

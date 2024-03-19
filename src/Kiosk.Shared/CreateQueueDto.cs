namespace Kiosk.Shared;

public record CreateQueueDto(
    string FullName,
    string UserName,
    string Email,
    string GradeAndSection,
    string Purpose,
    string Card,
    string Forms,
    string Others);

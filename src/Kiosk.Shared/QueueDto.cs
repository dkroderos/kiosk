namespace Kiosk.Shared;

public record QueueDto(
    int Id,
    string FullName,
    string UserName,
    string Email,
    string GradeAndSection,
    string Purpose,
    string Card,
    string Forms,
    string Others,
    string DateRegistered);
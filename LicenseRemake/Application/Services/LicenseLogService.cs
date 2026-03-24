using LicenseRemake.Application.Interfaces;
using LicenseRemake.Domain.Helpers;
using LicenseRemake.DTO.Licensing;
using LicenseRemake.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Application.Services;

public class LicenseLogService : ILicenseLogService
{
    private readonly DataDbContext _context;

    public LicenseLogService(DataDbContext context)
    {
        _context = context;
    }

    public async Task<LicenseLogSearchResult> SearchAsync(
        LicenseLogSearchRequest request,
        CancellationToken cancellationToken)
    {
        var query = _context.LicenseLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SerialNumber))
            query = query.Where(x => x.FcNumber == request.SerialNumber);

        if (request.DateFrom.HasValue)
            query = query.Where(x => x.DateIssue >= DateUtils.ToUtc(request.DateFrom.Value));

        if (request.DateTo.HasValue)
            query = query.Where(x => x.DateIssue < DateUtils.ToUtc(request.DateTo.Value));

        var total = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.DateIssue)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new LicenseLogDto(
                x.FcNumber,
                x.Host,
                DateUtils.ToKyiv(x.DateIssue),
                DateUtils.ToKyiv(x.DateExpired),
                x.Signature))
            .ToListAsync(cancellationToken);

        return new LicenseLogSearchResult(
            total,
            request.Page,
            request.PageSize,
            data);
    }
}

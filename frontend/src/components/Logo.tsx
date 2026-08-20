export function LogoMark({ size = 32, className = '' }: { size?: number; className?: string }) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 40 40"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      className={className}
      role="img"
      aria-label="Autonomiza"
    >
      <defs>
        <linearGradient id="autonomiza-logo-grad" x1="0" y1="0" x2="40" y2="40" gradientUnits="userSpaceOnUse">
          <stop stopColor="#6366f1" />
          <stop offset="1" stopColor="#4338ca" />
        </linearGradient>
      </defs>
      <rect x="1" y="1" width="38" height="38" rx="10" fill="url(#autonomiza-logo-grad)" />
      <path d="M12 29 20 11 28 29" stroke="#fff" strokeWidth="4" strokeLinecap="round" strokeLinejoin="round" />
      <line x1="15.5" y1="23" x2="24.5" y2="23" stroke="#fff" strokeWidth="4" strokeLinecap="round" />
      <circle cx="20" cy="7.5" r="2.4" fill="#fbbf24" />
    </svg>
  );
}

export function Logo({ size = 32, className = '' }: { size?: number; className?: string }) {
  return (
    <div className={`flex items-center gap-2 ${className}`}>
      <LogoMark size={size} />
      <span className="text-lg font-bold text-slate-900">Autonomiza</span>
    </div>
  );
}

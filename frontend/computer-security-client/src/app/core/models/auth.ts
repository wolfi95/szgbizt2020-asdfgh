

export interface TokenModel {
    accessToken?: string;
    refreshToken?: string;
}

export interface TokenLoginModel {
    accessToken: string;
    refreshToken: string;
    Email: string;
    RoleName: string;
    UserName: string;
    IFPSClaim: string[];
    UserId: string;
}

export interface LoginViewModel {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface JwtTokenModel {
    Email: string;
    ImageContainerName: string;
    ImageFileName: string;
    Language: string;
    RoleName: string;
    UserName: string;
    IFPSClaim: string[];
    UserId: string;
    CompanyId: string;
}

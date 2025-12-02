import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { Auth, LoginRequest, RegisterRequest, UserDto } from './auth';
import { Storage } from './storage';

describe('Auth', () => {
  let service: Auth;
  let httpMock: HttpTestingController;
  let mockRouter: jest.Mocked<Router>;
  let mockStorage: jest.Mocked<Storage>;

  const mockUser: UserDto = {
    id: '1',
    email: 'test@example.com',
    displayName: 'Test User',
    createdAt: '2023-01-01T00:00:00Z',
    token: 'mock-token',
  };

  beforeEach(() => {
    mockRouter = {
      navigate: jest.fn(),
    } as any;

    mockStorage = {
      getToken: jest.fn(),
      getUser: jest.fn(),
      saveToken: jest.fn(),
      saveUser: jest.fn(),
      removeToken: jest.fn(),
      removeUser: jest.fn(),
      clear: jest.fn(),
    } as any;

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        Auth,
        { provide: Router, useValue: mockRouter },
        { provide: Storage, useValue: mockStorage },
      ],
    });

    service = TestBed.inject(Auth);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should send a POST request and handle authentication success', () => {
      const loginRequest: LoginRequest = {
        email: 'test@example.com',
        password: 'password',
      };

      service.login(loginRequest).subscribe((user) => {
        expect(user).toEqual(mockUser);
        expect(mockStorage.saveToken).toHaveBeenCalledWith(mockUser.token);
        expect(mockStorage.saveUser).toHaveBeenCalledWith(mockUser);
        expect(service.currentUser()).toEqual(mockUser);
        expect(service.isAuthenticated()).toBe(true);
      });

      const req = httpMock.expectOne('http://localhost:5287/api/auth/login');
      expect(req.request.method).toBe('POST');
      req.flush(mockUser);
    });
  });

  describe('register', () => {
    it('should send a POST request and handle authentication success', () => {
      const registerRequest: RegisterRequest = {
        email: 'test@example.com',
        password: 'password',
        displayName: 'Test User',
      };

      service.register(registerRequest).subscribe((user) => {
        expect(user).toEqual(mockUser);
        expect(mockStorage.saveToken).toHaveBeenCalledWith(mockUser.token);
        expect(mockStorage.saveUser).toHaveBeenCalledWith(mockUser);
        expect(service.currentUser()).toEqual(mockUser);
        expect(service.isAuthenticated()).toBe(true);
      });

      const req = httpMock.expectOne('http://localhost:5287/api/auth/register');
      expect(req.request.method).toBe('POST');
      req.flush(mockUser);
    });
  });

  describe('logout', () => {
    it('should clear storage, reset signals, and navigate to login', () => {
      service.logout();

      expect(mockStorage.removeToken).toHaveBeenCalled();
      expect(mockStorage.removeUser).toHaveBeenCalled();
      expect(service.currentUser()).toBeNull();
      expect(service.isAuthenticated()).toBe(false);
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
    });
  });

  describe('getToken', () => {
    it('should return the token from storage', () => {
      mockStorage.getToken.mockReturnValue('mock-token');
      expect(service.getToken()).toBe('mock-token');
    });
  });

  describe('isTokenExpired', () => {
    it('should return true if no token is present', () => {
      mockStorage.getToken.mockReturnValue(null);
      expect(service.isTokenExpired()).toBe(true);
    });

    it('should return true if the token is expired', () => {
      const expiredToken = btoa(
        JSON.stringify({ exp: Math.floor(Date.now() / 1000) - 10 })
      );
      mockStorage.getToken.mockReturnValue(`header.${expiredToken}.signature`);
      expect(service.isTokenExpired()).toBe(true);
    });

    it('should return false if the token is not expired', () => {
      const validToken = btoa(
        JSON.stringify({ exp: Math.floor(Date.now() / 1000) + 1000 })
      );
      mockStorage.getToken.mockReturnValue(`header.${validToken}.signature`);
      expect(service.isTokenExpired()).toBe(false);
    });

    it('should return true if token parsing fails', () => {
      mockStorage.getToken.mockReturnValue('invalid-token');
      expect(service.isTokenExpired()).toBe(true);
    });
  });

  describe('loadUserFromStorage', () => {
    it('should load user and set signals if token and user exist', () => {
      mockStorage.getToken.mockReturnValue('mock-token');
      mockStorage.getUser.mockReturnValue(mockUser);

      (service as any).loadUserFromStorage();

      expect(service.currentUser()).toEqual(mockUser);
      expect(service.isAuthenticated()).toBe(true);
    });

    it('should not set signals if token or user is missing', () => {
      mockStorage.getToken.mockReturnValue(null);
      mockStorage.getUser.mockReturnValue(null);

      (service as any).loadUserFromStorage();

      expect(service.currentUser()).toBeNull();
      expect(service.isAuthenticated()).toBe(false);
    });
  });
});
